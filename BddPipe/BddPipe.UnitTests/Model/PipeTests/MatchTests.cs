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
                default(Pipe<int>).Match(v => v, e => DefaultValue);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
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

            var fnT = Substitute.For<Action<int>>();
            var fnError = Substitute.For<Action<ExceptionDispatchInfo>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Is(DefaultValue));
            fnError.DidNotReceive();
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

            var fnT = Substitute.For<Action<int>>();
            var fnError = Substitute.For<Action<ExceptionDispatchInfo>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<ExceptionDispatchInfo>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Action<ExceptionDispatchInfo>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
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

            var fnT = Substitute.For<Action<int>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValue_CallsFuncCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<int, Unit>>();
            var fnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Is(DefaultValue));
            fnError.DidNotReceive();
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

            var fnT = Substitute.For<Func<int, Unit>>();
            var fnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<ExceptionDispatchInfo>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValue_ReturnsFuncOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<ExceptionDispatchInfo, string>>();

            const string resultText = "some result";
            var result = pipe.Match(ctnInt => resultText, fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnError_ReturnsFuncOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnT = Substitute.For<Func<int, string>>();
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            const string resultText = "some result";
            var result = pipe.Match(fnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
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

            var fnT = Substitute.For<Func<int, Unit>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }
    }
}
