using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    internal sealed class ScenarioDetails
    {
        public string Value { get; }
        public ExceptionDispatchInfo ExceptionDispatchInfo { get; }
        public string ScenarioTitle { get; }
        public IReadOnlyList<StepOutcome> StepOutcomes { get; }

        public ScenarioDetails(
            string value,
            ExceptionDispatchInfo exceptionDispatchInfo,
            string scenarioTitle,
            IReadOnlyList<StepOutcome> stepOutcomes)
        {
            Value = value;
            ScenarioTitle = scenarioTitle;
            StepOutcomes = stepOutcomes;
            ExceptionDispatchInfo = exceptionDispatchInfo;
        }
    }

    internal static class PipeTestsHelper
    {
        public static ScenarioDetails GetDefaultScenarioDetails()
        {
            const string someText = "some text";
            const string scenarioTitle = "Scenario title";
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "Step 1"),
                new StepOutcome(Step.And, Outcome.Fail, "Step 2")
            };

            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            return new ScenarioDetails(someText, exInfo, scenarioTitle, stepOutcomes);
        }

        public static void ShouldHaveStepResultsAsDefaultScenarioDetails(this ScenarioResult scenarioResult)
        {
            scenarioResult.Should().NotBeNull();
            scenarioResult.Title.Should().Be("Scenario title");
            scenarioResult.Description.Should().Be("Scenario: Scenario title");
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(2);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "Step 1", "  Given Step 1 [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Fail, "Step 2", "    And Step 2 [Failed]", Step.And, 1);
        }

        public static Pipe<T> CreatePipe<T>(bool fromTask, T value, IReadOnlyList<StepOutcome> stepOutcomes, string scenarioTitle)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctn = new Ctn<T>(value, stepOutcomes, scenarioTitle);

            return fromTask
                ? new Pipe<T>(Task.FromResult(ctn))
                : new Pipe<T>(ctn);
        }

        public static Pipe<T> CreatePipe<T>(bool fromTask, T value) =>
            fromTask
                ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<T>(value, None)))
                : new Pipe<T>(new Ctn<T>(value, None));

        public static Pipe<T> CreatePipeErrorState<T>(bool fromTask, ExceptionDispatchInfo exDispatchInfo, IReadOnlyList<StepOutcome> stepOutcomes, string scenarioTitle)
        {
            var exInfo = exDispatchInfo ?? ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctn = new Ctn<ExceptionDispatchInfo>(exInfo, stepOutcomes, scenarioTitle);

            return
                fromTask
                    ? new Pipe<T>(Task.FromResult(ctn))
                    : new Pipe<T>(ctn);
        }

        public static Pipe<T> CreatePipeErrorState<T>(bool fromTask, ExceptionDispatchInfo exDispatchInfo = null)
        {
            var exInfo = exDispatchInfo ?? ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            return
                fromTask
                    ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<ExceptionDispatchInfo>(exInfo, None)))
                    : new Pipe<T>(new Ctn<ExceptionDispatchInfo>(exInfo, None));
        }
    }
}
