using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class PipeTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void CtorCtnException_WithCtnException_IsInErrorState()
        {
            var ex = new ApplicationException("test message");
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            Ctn<ExceptionDispatchInfo> value = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeError(error =>
            {
                error.Content.Should().Be(exInfo);
            });
        }

        [Test]
        public void CtorCtnValue_WithCtnValue_IsInValueState()
        {
            Ctn<int> value = new Ctn<int>(DefaultValue, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(DefaultValue);
            });
        }

        [Test]
        public void Match_DefaultPipe_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                default(Pipe<int>).Match(v => v.Content, e => DefaultValue);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public async Task MatchAsync_DefaultPipe_ThrowsNotInitializedException()
        {
            Func<Task> call = async () =>
            {
                await default(Pipe<int>).MatchAsync(v => v.Content, e => DefaultValue);
            };

            (await call.Should().ThrowExactlyAsync<PipeNotInitializedException>())
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionCtnDefaultValue_CallsActionCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);

            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();
            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionCtnError_CallsActionCtnError(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();
            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            Action call = () => pipe.Match(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionCtnErrorNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();

            Action call = () => pipe.Match(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");

            fnCtnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValue_CallsFuncCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnDefaultValue_CallsFuncCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            await pipe.MatchAsync(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnError_CallsFuncCtnError(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnError_CallsFuncCtnError(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            await pipe.MatchAsync(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValue_ReturnsFuncOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, string>>();

            const string resultText = "some result";
            var result = pipe.Match(ctnInt => resultText, fnCtnError);

            result.Should().Be(resultText);

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnDefaultValue_ReturnsFuncOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, string>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(ctnInt => resultText, fnCtnError);

            result.Should().Be(resultText);

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnError_ReturnsFuncOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, string>>();
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            const string resultText = "some result";
            var result = pipe.Match(fnCtnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnCtnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnError_ReturnsFuncOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, string>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(fnCtnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnCtnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            Action call = () => pipe.Match(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            Func<Task> call = () => pipe.MatchAsync(null, fnCtnError);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnErrorNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();

            Action call = () => pipe.Match(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");

            fnCtnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncCtnErrorNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();

            Func<Task> call = () => pipe.MatchAsync(fnCtnT, null);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("containerOfError");

            fnCtnT.DidNotReceive();
        }
    }
}
