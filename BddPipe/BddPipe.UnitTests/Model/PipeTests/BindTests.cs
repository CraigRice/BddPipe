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
        private static Pipe<T> CreatePipe<T>(T value, bool fromTask) =>
            fromTask
                ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<T>(value, None)))
                : new Pipe<T>(new Ctn<T>(value, None));

        private static Pipe<T> CreatePipeErrorState<T>(bool fromTask)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            return
                fromTask
                ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<ExceptionDispatchInfo>(exInfo, None)))
                : new Pipe<T>(new Ctn<ExceptionDispatchInfo>(exInfo, None));
        }

        public Func<PipeState<string>, Pipe<int>> FnBindStringLength(string text)
        {
            var fn = Substitute.For<Func<PipeState<string>, Pipe<int>>>();
            fn(Arg.Is<PipeState<string>>(state => state.Value == text)).Returns(CreatePipe(text.Length, false));
            return fn;
        }

        public Func<PipeState<string>, Task<Pipe<int>>> FnBindStringLengthAsync(string text)
        {
            var fn = Substitute.For<Func<PipeState<string>, Task<Pipe<int>>>>();
            fn(Arg.Is<PipeState<string>>(state => state.Value == text)).Returns(Task.FromResult(CreatePipe(text.Length, false)));
            return fn;
        }

        [Test]
        public void Bind_DefaultPipeWithSyncFunc_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                var fn = Substitute.For<Func<PipeState<int>, Pipe<string>>>();
                default(Pipe<int>).Bind(fn);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void Bind_DefaultPipeWithAsyncFunc_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                var fn = Substitute.For<Func<PipeState<int>, Task<Pipe<string>>>>();
                default(Pipe<int>).Bind(fn);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Bind_FnBindStringLength_ReturnsPipe(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipe(someText, fromTask)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            fn.Received()(Arg.Is<PipeState<string>>(state => state.Value == someText));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Bind_FnBindStringLengthAsync_ReturnsPipe(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipe(someText, fromTask)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            await fn.Received()(Arg.Is<PipeState<string>>(state => state.Value == someText));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Bind_ErrorStateFnBindStringLength_HasErrorStateNoFuncRun(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipeErrorState<string>(fromTask)
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            fn.DidNotReceiveWithAnyArgs()(Arg.Any<PipeState<string>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Bind_ErrorStateFnBindStringLengthAsync_HasErrorStateNoFuncRun(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipeErrorState<string>(fromTask)
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            await fn.DidNotReceiveWithAnyArgs()(Arg.Any<PipeState<string>>());
        }
    }
}
