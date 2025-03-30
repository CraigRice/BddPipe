using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static BddPipe.F;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class StepResultExtensionsTests
    {
        [Test]
        public void WithLatestStepOutcomeAsFail_StepOutcomesEmpty_ReturnsEmpty()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>();

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().BeEmpty();
        }

        [Test]
        public void WithLatestStepOutcomeAsFail_StepOutcomesSingleFail_ReturnsSingleFail()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Fail, "step-text")
            };

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].Step.Should().Be(Step.Given);
            result[0].Outcome.Should().Be(Outcome.Fail);
            result[0].Text.ShouldBeSome(txt => txt.Should().Be("step-text"));
        }

        [Test]
        public void WithLatestStepOutcomeAsFail_TextIsNone_ReturnsTextAsNone()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.NotRun, None)
            };

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].Step.Should().Be(Step.Given);
            result[0].Outcome.Should().Be(Outcome.Fail);
            result[0].Text.ShouldBeNone();
        }

        [Test]
        public void WithLatestStepOutcomeAsFail_StepOutcomesLastOfTwoIsFail_ReturnsLastOfTwoFail()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "given-text"),
                new StepOutcome(Step.When, Outcome.Fail, "when-text")
            };

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Step.Should().Be(Step.Given);
            result[0].Outcome.Should().Be(Outcome.Pass);
            result[0].Text.ShouldBeSome(txt => txt.Should().Be("given-text"));
            result[1].Step.Should().Be(Step.When);
            result[1].Outcome.Should().Be(Outcome.Fail);
            result[1].Text.ShouldBeSome(txt => txt.Should().Be("when-text"));
        }

        [Test]
        public void WithLatestStepOutcomeAsFail_StepOutcomesSinglePass_ReturnsSingleFail()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "step-text")
            };

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].Step.Should().Be(Step.Given);
            result[0].Outcome.Should().Be(Outcome.Fail);
            result[0].Text.ShouldBeSome(txt => txt.Should().Be("step-text"));
        }

        [Test]
        public void WithLatestStepOutcomeAsFail_StepOutcomesLastOfTwoIsPass_ReturnsLastOfTwoFail()
        {
            IReadOnlyList<StepOutcome> outcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "given-text"),
                new StepOutcome(Step.When, Outcome.Pass, "when-text")
            };

            var result = outcomes.WithLatestStepOutcomeAs(Outcome.Fail);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Step.Should().Be(Step.Given);
            result[0].Outcome.Should().Be(Outcome.Pass);
            result[0].Text.ShouldBeSome(txt => txt.Should().Be("given-text"));
            result[1].Step.Should().Be(Step.When);
            result[1].Outcome.Should().Be(Outcome.Fail);
            result[1].Text.ShouldBeSome(txt => txt.Should().Be("when-text"));
        }

        [Test]
        public void ToOutcome_Exception_Fail()
        {
            StepResultExtensions.ToOutcome(new Exception()).Should().Be(Outcome.Fail);
        }

        [Test]
        public void ToOutcome_FormatException_Fail()
        {
            StepResultExtensions.ToOutcome(new FormatException()).Should().Be(Outcome.Fail);
        }

        [Test]
        public void ToOutcome_InconclusiveException_Inconclusive()
        {
            StepResultExtensions.ToOutcome(new InconclusiveException("test message")).Should().Be(Outcome.Inconclusive);
        }

        [Test]
        public void ToTitle_NullTitleString_ReturnsCorrectTitle()
        {
            string? titleText = null;

            var result = titleText.ToTitle(Step.And).Value;

            result.Step.Should().Be(Step.And);
            result.Text.ShouldBeNone();
        }

        [Test]
        public void ToTitle_TitleString_ReturnsCorrectTitle()
        {
            const string titleText = "title text";

            var result = titleText.ToTitle(Step.And).Value;

            result.Step.Should().Be(Step.And);
            result.Text.ShouldBeSome(title => title.Should().Be(titleText));
        }

        [Test]
        public void ToStepOutcome_TitleAndSuppliedOutcome_ReturnsMappedStepOutcome()
        {
            const string titleText = "title text";

            Some<Title> title = new Title(Step.And, titleText);

            var stepOutcome = title.ToStepOutcome(Outcome.Inconclusive);

            stepOutcome.Should().NotBeNull();
            stepOutcome.Step.Should().Be(Step.And);
            stepOutcome.Outcome.Should().Be(Outcome.Inconclusive);
            stepOutcome.Text.ShouldBeSome(titleOptionValue => titleOptionValue.Should().Be(titleText));
        }

        [Test]
        public void ToStepOutcome_NullTitleAndSuppliedOutcome_ReturnsMappedStepOutcome()
        {
            Some<Title> title = new Title(Step.And, None);

            var stepOutcome = title.ToStepOutcome(Outcome.Inconclusive);

            stepOutcome.Should().NotBeNull();
            stepOutcome.Step.Should().Be(Step.And);
            stepOutcome.Outcome.Should().Be(Outcome.Inconclusive);
            stepOutcome.Text.ShouldBeNone();
        }

        [Test]
        public void ToResults_Empty_ReturnsEmpty()
        {
            IReadOnlyList<StepOutcome> stepResults = new List<StepOutcome>();
            var results = stepResults.ToResults(false);

            results.Should().NotBeNull();
            results.Should().BeEmpty();
        }

        private const string StepTitle = "step title";

        [TestCase(false, Step.Given, Outcome.Fail, StepTitle, "Given step title [Failed]")]
        [TestCase(false, Step.Given, Outcome.Inconclusive, StepTitle, "Given step title [Inconclusive]")]
        [TestCase(false, Step.Given, Outcome.NotRun, StepTitle, "Given step title [not run]")]
        [TestCase(false, Step.Given, Outcome.Pass, StepTitle, "Given step title [Passed]")]

        [TestCase(false, Step.When, Outcome.Fail, StepTitle, "When step title [Failed]")]
        [TestCase(false, Step.When, Outcome.Inconclusive, StepTitle, "When step title [Inconclusive]")]
        [TestCase(false, Step.When, Outcome.NotRun, StepTitle, "When step title [not run]")]
        [TestCase(false, Step.When, Outcome.Pass, StepTitle, "When step title [Passed]")]

        [TestCase(false, Step.Then, Outcome.Fail, StepTitle, "Then step title [Failed]")]
        [TestCase(false, Step.Then, Outcome.Inconclusive, StepTitle, "Then step title [Inconclusive]")]
        [TestCase(false, Step.Then, Outcome.NotRun, StepTitle, "Then step title [not run]")]
        [TestCase(false, Step.Then, Outcome.Pass, StepTitle, "Then step title [Passed]")]

        [TestCase(false, Step.And, Outcome.Fail, StepTitle, "  And step title [Failed]")]
        [TestCase(false, Step.And, Outcome.Inconclusive, StepTitle, "  And step title [Inconclusive]")]
        [TestCase(false, Step.And, Outcome.NotRun, StepTitle, "  And step title [not run]")]
        [TestCase(false, Step.And, Outcome.Pass, StepTitle, "  And step title [Passed]")]

        [TestCase(false, Step.But, Outcome.Fail, StepTitle, "  But step title [Failed]")]
        [TestCase(false, Step.But, Outcome.Inconclusive, StepTitle, "  But step title [Inconclusive]")]
        [TestCase(false, Step.But, Outcome.NotRun, StepTitle, "  But step title [not run]")]
        [TestCase(false, Step.But, Outcome.Pass, StepTitle, "  But step title [Passed]")]

        [TestCase(true, Step.Given, Outcome.Fail, StepTitle, "  Given step title [Failed]")]
        [TestCase(true, Step.Given, Outcome.Inconclusive, StepTitle, "  Given step title [Inconclusive]")]
        [TestCase(true, Step.Given, Outcome.NotRun, StepTitle, "  Given step title [not run]")]
        [TestCase(true, Step.Given, Outcome.Pass, StepTitle, "  Given step title [Passed]")]

        [TestCase(true, Step.When, Outcome.Fail, StepTitle, "  When step title [Failed]")]
        [TestCase(true, Step.When, Outcome.Inconclusive, StepTitle, "  When step title [Inconclusive]")]
        [TestCase(true, Step.When, Outcome.NotRun, StepTitle, "  When step title [not run]")]
        [TestCase(true, Step.When, Outcome.Pass, StepTitle, "  When step title [Passed]")]

        [TestCase(true, Step.Then, Outcome.Fail, StepTitle, "  Then step title [Failed]")]
        [TestCase(true, Step.Then, Outcome.Inconclusive, StepTitle, "  Then step title [Inconclusive]")]
        [TestCase(true, Step.Then, Outcome.NotRun, StepTitle, "  Then step title [not run]")]
        [TestCase(true, Step.Then, Outcome.Pass, StepTitle, "  Then step title [Passed]")]

        [TestCase(true, Step.And, Outcome.Fail, StepTitle, "    And step title [Failed]")]
        [TestCase(true, Step.And, Outcome.Inconclusive, StepTitle, "    And step title [Inconclusive]")]
        [TestCase(true, Step.And, Outcome.NotRun, StepTitle, "    And step title [not run]")]
        [TestCase(true, Step.And, Outcome.Pass, StepTitle, "    And step title [Passed]")]

        [TestCase(true, Step.But, Outcome.Fail, StepTitle, "    But step title [Failed]")]
        [TestCase(true, Step.But, Outcome.Inconclusive, StepTitle, "    But step title [Inconclusive]")]
        [TestCase(true, Step.But, Outcome.NotRun, StepTitle, "    But step title [not run]")]
        [TestCase(true, Step.But, Outcome.Pass, StepTitle, "    But step title [Passed]")]
        public void ToResults_IndividualForEachStepAndOutcomeAndScenario_MapsToExpected(bool hasScenario, Step step, Outcome outcome, string stepTitleValue, string expected)
        {
            IReadOnlyList<StepOutcome> stepResults = new List<StepOutcome>
            {
                new StepOutcome(step, outcome, stepTitleValue)
            };

            var results = stepResults.ToResults(hasScenario);

            results.Should().NotBeNull();
            results.Count.Should().Be(1);
            var result = results[0];
            result.Title.Should().Be(stepTitleValue);
            result.Outcome.Should().Be(outcome);
            result.Description.Should().Be(expected);
            result.Step.Should().Be(step);
        }

        [TestCase(StepTitle)]
        [TestCase(null)]
        public void ToResults_TitleWithValueAndNone_MapsTitle(string stepTitleValue)
        {
            Option<string> stepTitleOption = stepTitleValue;

            IReadOnlyList<StepOutcome> stepResults = new List<StepOutcome>
            {
                new StepOutcome(Step.But, Outcome.Pass, stepTitleOption)
            };

            var results = stepResults.ToResults(false);

            results.Should().NotBeNull();
            results.Count.Should().Be(1);
            var result = results[0];
            result.Title.Should().Be(stepTitleValue);
        }
    }
}
