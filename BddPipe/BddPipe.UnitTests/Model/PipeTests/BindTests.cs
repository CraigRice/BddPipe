using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using static BddPipe.F;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public partial class BindTests
    {
        private static Pipe<T> CreatePipe<T>(T value) =>
            new Pipe<T>(new Ctn<T>(value, None));

        private static Pipe<T> CreatePipeErrorState<T>()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            return new Pipe<T>(new Ctn<ExceptionDispatchInfo>(exInfo, None));
        }

        public Func<string, Pipe<int>> FnBindStringLength(string text)
        {
            var fn = Substitute.For<Func<string, Pipe<int>>>();
            fn(text).Returns(CreatePipe(text.Length));
            return fn;
        }

        public Func<string, Task<Pipe<int>>> FnBindStringLengthAsync(string text)
        {
            var fn = Substitute.For<Func<string, Task<Pipe<int>>>>();
            fn(text).Returns(Task.FromResult(CreatePipe(text.Length)));
            return fn;
        }

        [Test]
        public void Bind_FnBindStringLength_ReturnsPipe()
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipe(someText)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            fn.Received()(someText);
        }

        [Test]
        public async Task Bind_FnBindStringLengthAsync_ReturnsPipe()
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipe(someText)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            await fn.Received()(someText);
        }

        [Test]
        public void Bind_ErrorStateFnBindStringLength_HasErrorStateNoFuncRun()
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipeErrorState<string>()
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            fn.DidNotReceiveWithAnyArgs()(Arg.Any<string>());
        }

        [Test]
        public async Task Bind_ErrorStateFnBindStringLengthAsync_HasErrorStateNoFuncRun()
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipeErrorState<string>()
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            await fn.DidNotReceiveWithAnyArgs()(Arg.Any<string>());
        }
    }
}
