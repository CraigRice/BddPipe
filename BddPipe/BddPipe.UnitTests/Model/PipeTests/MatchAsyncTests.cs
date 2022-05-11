using System;
using System.Threading.Tasks;
using BddPipe.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.UnitTests.Model.PipeTests.PipeTestsHelper;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchAsyncTests
    {
        private const int DefaultValue = 45;

        [Test]
        public async Task MatchAsync_DefaultPipe_ThrowsNotInitializedException()
        {
            Func<Task> call = async () =>
            {
                await default(Pipe<int>).MatchAsync(pipeState => Task.FromResult(pipeState.Value), e => Task.FromResult(DefaultValue));
            };

            (await call.Should().ThrowExactlyAsync<PipeNotInitializedException>())
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public async Task MatchAsync_SyncFnDefaultPipe_ThrowsNotInitializedException()
        {
            Func<Task> call = async () =>
            {
                await default(Pipe<int>).MatchAsync(pipeState => pipeState.Value, e => DefaultValue);
            };

            (await call.Should().ThrowExactlyAsync<PipeNotInitializedException>())
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_CallsFuncRight(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnT = Substitute.For<Func<PipeData<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorData, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            await fnT.Received()(Arg.Is<PipeData<int>>(state => state.Value == DefaultValue));
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncRight_CallsFuncRight(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnT = Substitute.For<Func<PipeData<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorData, Unit>>();

            await pipe.MatchAsync(fnT, fnError);

            fnT.Received()(Arg.Is<PipeData<int>>(state => state.Value == DefaultValue));
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_CallsFuncLeft(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorData, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            fnT.DidNotReceive();
            await fnError.Received()(Arg.Any<PipeErrorData>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncLeft_CallsFuncLeft(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorData, Unit>>();

            await pipe.MatchAsync(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<PipeErrorData>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnError = Substitute.For<Func<PipeErrorData, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(value => Task.FromResult(resultText), fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncRight_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnError = Substitute.For<Func<PipeErrorData, string>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(value => resultText, fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(fnT, value => Task.FromResult(resultText));

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncLeft_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, string>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(fnT, value => resultText);

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRightNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnError = Substitute.For<Func<PipeErrorData, Task<Unit>>>();

            Func<Task> call = () => pipe.MatchAsync(null, fnError);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncRightNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipe(fromTask, DefaultValue);

            var fnError = Substitute.For<Func<PipeErrorData, Unit>>();

            Func<Task> call = () => pipe.MatchAsync(null, fnError);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeftNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, Task<Unit>>>();

            Func<Task> call = () => pipe.MatchAsync(fnT, null);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithFuncLeftNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeData<int>, Unit>>();

            Func<Task> call = () => pipe.MatchAsync(fnT, null);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithScenarioData_MapArgIsPopulated(bool fromTask)
        {
            var scenarioDetails = GetDefaultScenarioDetails();

            var pipe = CreatePipe(fromTask, scenarioDetails.Value, scenarioDetails.StepOutcomes, scenarioDetails.ScenarioTitle);

            await pipe.MatchAsync(
                state =>
                {
                    state.Should().NotBeNull();
                    state.Value.Should().Be(scenarioDetails.Value);
                    state.Result.ShouldHaveStepResultsAsDefaultScenarioDetails();
                    return Task.FromResult(new Unit());
                },
                error =>
                {
                    Assert.Fail("Expecting value state but was error state.");
                    return Task.FromResult(new Unit());
                });
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithScenarioData_MapArgIsPopulated(bool fromTask)
        {
            var scenarioDetails = GetDefaultScenarioDetails();

            var pipe = CreatePipe(fromTask, scenarioDetails.Value, scenarioDetails.StepOutcomes, scenarioDetails.ScenarioTitle);

            await pipe.MatchAsync(
                state =>
                {
                    state.Should().NotBeNull();
                    state.Value.Should().Be(scenarioDetails.Value);
                    state.Result.ShouldHaveStepResultsAsDefaultScenarioDetails();
                    return new Unit();
                },
                error =>
                {
                    Assert.Fail("Expecting value state but was error state.");
                    return new Unit();
                });
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithExceptionData_MapArgIsPopulated(bool fromTask)
        {
            var scenarioDetails = GetDefaultScenarioDetails();

            var pipe = CreatePipeErrorState<int>(fromTask, scenarioDetails.ExceptionDispatchInfo, scenarioDetails.StepOutcomes, scenarioDetails.ScenarioTitle);

            await pipe.MatchAsync(
                state =>
                {
                    Assert.Fail("Expecting error state but was value state.");
                    return new Unit();
                },
                error =>
                {
                    error.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().Be(scenarioDetails.ExceptionDispatchInfo);
                    error.Result.ShouldHaveStepResultsAsDefaultScenarioDetails();
                    return new Unit();
                });
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_SyncFnWithExceptionData_MapArgIsPopulated(bool fromTask)
        {
            var scenarioDetails = GetDefaultScenarioDetails();

            var pipe = CreatePipeErrorState<int>(fromTask, scenarioDetails.ExceptionDispatchInfo, scenarioDetails.StepOutcomes, scenarioDetails.ScenarioTitle);

            await pipe.MatchAsync(
                state =>
                {
                    Assert.Fail("Expecting error state but was value state.");
                    return new Unit();
                },
                error =>
                {
                    error.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().Be(scenarioDetails.ExceptionDispatchInfo);
                    error.Result.ShouldHaveStepResultsAsDefaultScenarioDetails();
                    return new Unit();
                });
        }
    }
}
