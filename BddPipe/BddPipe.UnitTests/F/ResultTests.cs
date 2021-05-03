using System;
using System.Runtime.ExceptionServices;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class ResultTests
    {
        private const int DefaultValue = 45;
        private static ExceptionDispatchInfo GetError() =>
            ExceptionDispatchInfo.Capture(new Exception("test"));

        [Test]
        public void Ctor_WithValue_IsSuccess()
        {
            var result = new Result<int>(DefaultValue);
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Ctor_WithValueViaImplicit_IsSuccess()
        {
            Result<int> result = DefaultValue;
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Ctor_WithError_IsFaulted()
        {
            var result = new Result<int>(GetError());
            result.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void Match_WithFuncCtnDefaultValue_CallsFuncCtnT()
        {
            var result = new Result<int>(DefaultValue);

            var fnCtnT = Substitute.For<Func<int, Unit>>();
            var fnCtnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            result.Match(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<int>());
            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnError_CallsFuncCtnError()
        {
            var result = new Result<int>(GetError());

            var fnCtnT = Substitute.For<Func<int, Unit>>();
            var fnCtnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            result.Match(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<ExceptionDispatchInfo>());
        }

        [Test]
        public void Match_WithFuncCtnDefaultValue_ReturnsFuncOutput()
        {
            var result = new Result<int>(DefaultValue);

            var fnCtnError = Substitute.For<Func<ExceptionDispatchInfo, string>>();

            const string resultText = "some result";
            var outcome = result.Match(ctnInt => resultText, fnCtnError);

            outcome.Should().Be(resultText);

            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnError_ReturnsFuncOutput()
        {
            var result = new Result<int>(GetError());

            var fnCtnT = Substitute.For<Func<int, string>>();

            const string resultText = "some result";
            var outcome = result.Match(fnCtnT, ctnError => resultText);

            outcome.Should().Be(resultText);

            fnCtnT.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnDefaultValueNull_ThrowsArgNullException()
        {
            var result = new Result<int>(DefaultValue);

            var fnCtnError = Substitute.For<Func<ExceptionDispatchInfo, Unit>>();

            Action call = () => result.Match(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnErrorNull_ThrowsArgNullException()
        {
            var result = new Result<int>(GetError());

            var fnCtnT = Substitute.For<Func<int, Unit>>();

            Action call = () => result.Match(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnCtnT.DidNotReceive();
        }
    }
}
