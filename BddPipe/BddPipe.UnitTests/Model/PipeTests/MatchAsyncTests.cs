using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
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

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_CallsFuncRight(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            await fnT.Received()(Arg.Is<PipeState<int>>(state => state.Value == DefaultValue));
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_CallsFuncLeft(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            fnT.DidNotReceive();
            await fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnError = Substitute.For<Func<PipeErrorState, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(value => Task.FromResult(resultText), fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_ReturnsFuncOutput(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(fnT, value => Task.FromResult(resultText));

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRightNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

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

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();

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
            const string someText = "some text";
            const string scenarioTitle = "Scenario title";
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "Step 1"),
                new StepOutcome(Step.And, Outcome.Fail, "Step 2")
            };

            var pipe = CreatePipe(someText, fromTask, stepOutcomes, scenarioTitle);

            await pipe.MatchAsync(
                state =>
                {
                    state.Should().NotBeNull();
                    state.Value.Should().Be(someText);
                    state.Result.Should().NotBeNull();
                    state.Result.Title.Should().Be(scenarioTitle);
                    state.Result.Description.Should().Be("Scenario: Scenario title");
                    state.Result.StepResults.Should().NotBeNull();
                    state.Result.StepResults.Count.Should().Be(stepOutcomes.Count);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "Step 1", "  Given Step 1 [Passed]", Step.Given, 0);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Fail, "Step 2", "    And Step 2 [Failed]", Step.And, 1);
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
        public void MatchAsync_WithExceptionData_MapArgIsPopulated(bool fromTask)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            var pipe = CreatePipeErrorState<int>(fromTask, exInfo);

            pipe.Match(
                state =>
                {
                    Assert.Fail("Expecting error state but was value state.");
                },
                error =>
                {
                    error.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().Be(exInfo);
                });
        }
    }
}
