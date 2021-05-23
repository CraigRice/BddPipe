using System;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class TryTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void Try_TryDelegateNull_ThrowsArgNullException()
        {
            Try<int> tryFunc = null;

            Action call = () => tryFunc.Try();

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public void Try_TryDelegateReturnsValue_ReturnsResultWithValue()
        {
            Try<int> tryFunc = () => DefaultValue;

            var result = tryFunc.Try();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            var resultValue = result.Match(val => val, err => 0);
            resultValue.Should().Be(DefaultValue);
        }

        [Test]
        public void Try_TryDelegateThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            Try<int> tryFunc = () => throw new ApplicationException(exMessage);

            var result = tryFunc.Try();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();

            result.Match(val =>
            {
                Assert.Fail($"Expecting ExceptionDispatchInfo but was '{val}'");
                return new Unit();
            }, err =>
            {
                err.Should().NotBeNull();
                err.SourceException.Should().BeOfType<ApplicationException>()
                    .Which
                    .Message.Should().Be(exMessage);

                return new Unit();
            });
        }

        [Test]
        public void TryRun_FuncRNull_ThrowsArgNullException()
        {
            Func<int> tryFunc = null;

            Action call = () => tryFunc.TryRun();

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public void TryRun_FuncRReturnsValue_ReturnsResultWithValue()
        {
            Func<int> tryFunc = () => DefaultValue;

            var result = tryFunc.TryRun();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            var resultValue = result.Match(val => val, err => 0);
            resultValue.Should().Be(DefaultValue);
        }

        [Test]
        public void TryRun_FuncRThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            Func<int> tryFunc = () => throw new ApplicationException(exMessage);

            var result = tryFunc.TryRun();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();

            result.Match(val =>
            {
                Assert.Fail($"Expecting ExceptionDispatchInfo but was '{val}'");
                return new Unit();
            }, err =>
            {
                err.Should().NotBeNull();
                err.SourceException.Should().BeOfType<ApplicationException>()
                    .Which
                    .Message.Should().Be(exMessage);

                return new Unit();
            });
        }
    }
}
