using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using BddPipe.UnitTests.Helpers;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class TryTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void Try_TryDelegateNull_ThrowsArgNullException()
        {
            Try<int> tryFunc = null!;

            Action call = () => tryFunc.Try();

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public async Task TryAsync_TryAsyncDelegateNull_ThrowsArgNullException()
        {
            TryAsync<int> tryFunc = null!;

            Func<Task> call = () => tryFunc.TryAsync();

            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public void Try_TryDelegateReturnsValue_ReturnsResultWithValue()
        {
            Try<int> tryFunc = () => DefaultValue;

            var result = tryFunc.Try();
            result.ShouldBeSuccessful(val => { val.Should().Be(DefaultValue); });
        }

        [Test]
        public async Task TryAsync_TryDelegateReturnsValue_ReturnsResultWithValue()
        {
            TryAsync<int> tryFunc = () => Task.FromResult(new Result<int>(DefaultValue));

            var result = await tryFunc.TryAsync();
            result.ShouldBeSuccessful(val => { val.Should().Be(DefaultValue); });
        }

        [Test]
        public void Try_TryDelegateThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            Try<int> tryFunc = () => throw new ApplicationException(exMessage);

            var result = tryFunc.Try();
            result.ShouldBeError(
                err =>
                {
                    err.SourceException.Should().BeOfType<ApplicationException>()
                        .Which
                        .Message.Should().Be(exMessage);
                });
        }

        [Test]
        public async Task TryAsync_TryDelegateThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            TryAsync<int> tryFunc = () => TestExceptions.Raise<Task<Result<int>>>(new ApplicationException(exMessage));

            var result = await tryFunc.TryAsync();
            result.ShouldBeError(
                err =>
                {
                    err.SourceException.Should().BeOfType<ApplicationException>()
                        .Which
                        .Message.Should().Be(exMessage);
            });
        }

        [Test]
        public void TryRun_FuncRNull_ThrowsArgNullException()
        {
            Func<int> tryFunc = null!;

            Action call = () => tryFunc.TryRun();

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public async Task TryRunAsync_FuncRNull_ThrowsArgNullException()
        {
            Func<Task<int>> tryFunc = null!;

            Func<Task> call = () => tryFunc.TryRunAsync();

            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("fn");
        }

        [Test]
        public void TryRun_FuncRReturnsValue_ReturnsResultWithValue()
        {
            Func<int> tryFunc = () => DefaultValue;

            var result = tryFunc.TryRun();
            result.ShouldBeSuccessful(val => { val.Should().Be(DefaultValue); });
        }

        [Test]
        public async Task TryRunAsync_FuncRReturnsValue_ReturnsResultWithValue()
        {
            Func<Task<int>> tryFunc = () => Task.FromResult(DefaultValue);

            var result = await tryFunc.TryRunAsync();
            result.ShouldBeSuccessful(val => { val.Should().Be(DefaultValue); });
        }

        [Test]
        public void TryRun_FuncRThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            Func<int> tryFunc = () => throw new ApplicationException(exMessage);

            var result = tryFunc.TryRun();
            result.ShouldBeError(err =>
            {
                err.SourceException.Should().BeOfType<ApplicationException>()
                    .Which
                    .Message.Should().Be(exMessage);
            });
        }

        [Test]
        public async Task TryRunAsync_FuncRThrowsException_ReturnsResultWithValue()
        {
            const string exMessage = "test ex message";
            Func<Task<int>> tryFunc = () => throw new ApplicationException(exMessage);

            var result = await tryFunc.TryRunAsync();
            result.ShouldBeError(err =>
            {
                err.SourceException.Should().BeOfType<ApplicationException>()
                    .Which
                    .Message.Should().Be(exMessage);
            });
        }
    }
}
