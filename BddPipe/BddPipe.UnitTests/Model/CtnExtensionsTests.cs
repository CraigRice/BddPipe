using System.Collections.Generic;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class CtnExtensionsTests
    {
        private const int DefaultValue = 32;
        private const string ScenarioTitle = "scenario-title";
        private const string GivenStepTitle = "given-step-title";
        private const string ThenStepTitle = "then-step-title";

        private Ctn<int> GetInitialCtn() =>
            new Ctn<int>(
                DefaultValue,
                new List<StepOutcome>
                {
                    new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle)
                },
                ScenarioTitle
            );

        [Test]
        public void ToCtn_OnStep_ReturnsNextCtn()
        {
            var initialCtn = GetInitialCtn();
            const string nextValue = "next-value";
            const string nextStepTitle = "next-step-title";

            var newCtn = initialCtn.ToCtn(nextValue, new StepOutcome(Step.And, Outcome.Fail, nextStepTitle));

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(nextValue);
            newCtn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(2);
            newCtn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, Step.Given, 0);
            newCtn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Fail, nextStepTitle, Step.And, 1);
        }

        [Test]
        public void ToResult_ScenarioTitleNone_TitleIsNullDescriptionIsPrefix()
        {
            var ctn = new Ctn<int>(DefaultValue, None);
            var scenarioResult = ctn.ToResult();
            scenarioResult.Should().NotBeNull();
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.Title.Should().BeNull();
            scenarioResult.Description.Should().Be("Scenario:");
        }

        [Test]
        public void ToResult_ScenarioTitleSet_TitleIsSetDescriptionIsSet()
        {
            var ctn = new Ctn<int>(DefaultValue, ScenarioTitle);
            var scenarioResult = ctn.ToResult();
            scenarioResult.Should().NotBeNull();
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.Title.Should().Be(ScenarioTitle);
            scenarioResult.Description.Should().Be($"Scenario: {ScenarioTitle}");
        }

        [Test]
        public void ToResult_StepOutcomesEmpty_StepResultsEmpty()
        {
            var ctn = new Ctn<int>(DefaultValue, None);
            var scenarioResult = ctn.ToResult();

            scenarioResult.Should().NotBeNull();
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEmpty();
        }

        [Test]
        public void ToResult_MultipleStepOutcomes_StepResultsMapped()
        {
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle),
                new StepOutcome(Step.When, Outcome.Inconclusive, None),
                new StepOutcome(Step.Then, Outcome.NotRun, ThenStepTitle)
            };

            var ctn = new Ctn<int>(DefaultValue, stepOutcomes, None);
            var scenarioResult = ctn.ToResult();

            scenarioResult.Should().NotBeNull();
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(3);

            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, $"{GivenStepTitle} [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, null, "When [Inconclusive]", Step.When, 1);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ThenStepTitle, $"{ThenStepTitle} [not run]", Step.Then, 2);
        }
    }
}
