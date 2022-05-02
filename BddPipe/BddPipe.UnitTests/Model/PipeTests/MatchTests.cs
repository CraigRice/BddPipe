using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void Match_DefaultPipe_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                default(Pipe<int>).Match(v => DefaultValue, e => DefaultValue);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionRight_CallsActionCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);

            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Action<PipeState<int>>>();
            var fnError = Substitute.For<Action<PipeErrorState>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Any<PipeState<int>>());
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionLeft_CallsActionCtnError(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Action<PipeState<int>>>();
            var fnError = Substitute.For<Action<PipeErrorState>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionRightNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Action<PipeErrorState>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionLeftNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Action<PipeState<int>>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRight_CallsFuncRight(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Any<PipeState<int>>());
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeft_CallsFuncLeft(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRight_ReturnsFuncRightOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<PipeErrorState, string>>();

            const string resultText = "some result";
            var result = pipe.Match(ctnInt => resultText, fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeft_ReturnsFuncLeftOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, string>>();

            const string resultText = "some result";
            var result = pipe.Match(fnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRightNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeftNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }
    }
}
