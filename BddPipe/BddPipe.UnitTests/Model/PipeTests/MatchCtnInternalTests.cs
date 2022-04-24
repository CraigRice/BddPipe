using System;
using System.Runtime.ExceptionServices;
using static BddPipe.F;
using System.Threading.Tasks;
using BddPipe.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchCtnInternalTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void MatchCtnInternal_DefaultPipe_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                default(Pipe<int>).MatchCtnInternal(v => v.Content, e => DefaultValue);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnDefaultValue_CallsFuncCtnT(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.MatchCtnInternal(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnError_CallsFuncCtnError(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.MatchCtnInternal(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnDefaultValue_ReturnsFuncOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, string>>();

            const string resultText = "some result";
            var result = pipe.MatchCtnInternal(ctnInt => resultText, fnCtnError);

            result.Should().Be(resultText);

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnError_ReturnsFuncOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, string>>();
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            const string resultText = "some result";
            var result = pipe.MatchCtnInternal(fnCtnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnCtnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnDefaultValueNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            Action call = () => pipe.MatchCtnInternal(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MatchCtnInternal_WithFuncCtnErrorNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();

            Action call = () => pipe.MatchCtnInternal(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfExceptionDispatchInfo");

            fnCtnT.DidNotReceive();
        }
    }
}
