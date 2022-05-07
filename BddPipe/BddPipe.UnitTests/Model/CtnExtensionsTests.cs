using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
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
        private const string AndStepTitle = "and-step-title";
        private const string ButStepTitle = "but-step-title";

        private Ctn<int> GetInitialCtnWithSuccessfulGiven() =>
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
            var initialCtn = GetInitialCtnWithSuccessfulGiven();
            const string nextValue = "next-value";
            const string nextStepTitle = "next-step-title";

            var newCtn = initialCtn.ToCtn(nextValue, new StepOutcome(Step.And, Outcome.Fail, nextStepTitle));

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(nextValue);
            newCtn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(2);
            newCtn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, GivenStepTitle, Step.Given, 0);
            newCtn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Fail, nextStepTitle, Step.And, 1);
        }

        [Test]
        public void Map_NullArgument_ThrowsArgumentNullException()
        {
            var initialCtn = GetInitialCtnWithSuccessfulGiven();
            Action map = () => { initialCtn.Map<int, string>(null); };

            map.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public async Task MapAsync_NullArgument_ThrowsArgumentNullException()
        {
            var initialCtn = GetInitialCtnWithSuccessfulGiven();
            Func<Task> map = async () =>
            {
                await initialCtn.MapAsync<int, string>(null);
            };

            (await map.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void Map_FromCtnIntToCtnBool_ChangesContent()
        {
            var fnMap = Substitute.For<Func<int, bool>>();
            fnMap(Arg.Any<int>()).Returns(true);

            var newCtn = new Ctn<int>(DefaultValue, None)
                .Map(fnMap);

            newCtn.Content.Should().BeTrue();
        }

        [Test]
        public async Task MapAsync_FromCtnIntToCtnBool_ChangesContent()
        {
            var fnMap = Substitute.For<Func<int, Task<bool>>>();
            fnMap(Arg.Any<int>()).Returns(Task.FromResult(true));

            var newCtn = await new Ctn<int>(DefaultValue, None)
                .MapAsync(fnMap);

            newCtn.Content.Should().BeTrue();
        }

        [Test]
        public void Map_ScenarioTitleNone_ScenarioTitleRemainsAsNone()
        {
            var newCtn = new Ctn<int>(DefaultValue, None)
                .Map(value => true);

            newCtn.ScenarioTitle.ShouldBeNone();
        }

        [Test]
        public async Task MapAsync_ScenarioTitleNone_ScenarioTitleRemainsAsNone()
        {
            var newCtn = await new Ctn<int>(DefaultValue, None)
                .MapAsync(async value => true);

            newCtn.ScenarioTitle.ShouldBeNone();
        }

        [Test]
        public void Map_ScenarioTitleSome_ScenarioTitleRemainsAsSome()
        {
            const string scenarioTitle = "scenario title value";

            var newCtn = new Ctn<int>(DefaultValue, scenarioTitle)
                .Map(value => true);

            newCtn.ScenarioTitle.ShouldBeSome(title => 
                title.Should().Be(scenarioTitle)
            );
        }

        [Test]
        public async Task MapAsync_ScenarioTitleSome_ScenarioTitleRemainsAsSome()
        {
            const string scenarioTitle = "scenario title value";

            var newCtn = await new Ctn<int>(DefaultValue, scenarioTitle)
                .MapAsync(async value => true);

            newCtn.ScenarioTitle.ShouldBeSome(title =>
                title.Should().Be(scenarioTitle)
            );
        }

        [Test]
        public void Map_OutcomesEmptyViaDefaultCtor_OutcomesRemainsAsEmpty()
        {
            var newCtn = new Ctn<int>(DefaultValue, None)
                .Map(value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public async Task MapAsync_OutcomesEmptyViaDefaultCtor_OutcomesRemainsAsEmpty()
        {
            var newCtn = await new Ctn<int>(DefaultValue, None)
                .MapAsync(async value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public void Map_OutcomesEmptyViaExplicitCtor_OutcomesRemainsAsEmpty()
        {
            var newCtn = new Ctn<int>(DefaultValue, new List<StepOutcome>(), None)
                .Map(value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public async Task MapAsync_OutcomesEmptyViaExplicitCtor_OutcomesRemainsAsEmpty()
        {
            var newCtn = await new Ctn<int>(DefaultValue, new List<StepOutcome>(), None)
                .MapAsync(async value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public void Map_OutcomesExist_OutcomesRemain()
        {
            var newCtn = new Ctn<int>(
                    DefaultValue,
                    new List<StepOutcome>
                    {
                        new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle)
                    }, 
                    None
                )
                .Map(value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, GivenStepTitle, Step.Given);
        }

        [Test]
        public async Task MapAsync_OutcomesExist_OutcomesRemain()
        {
            var newCtn = await new Ctn<int>(
                    DefaultValue,
                    new List<StepOutcome>
                    {
                        new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle)
                    },
                    None
                )
                .MapAsync(async value => true);

            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, GivenStepTitle, Step.Given);
        }

        [Test]
        public void Map_FromCtn_MapFunctionIsCalled()
        {
            var fnMap = Substitute.For<Func<int, bool>>();
            fnMap(Arg.Any<int>()).Returns(true);

            new Ctn<int>(DefaultValue, None)
                .Map(fnMap);

            fnMap.Received()(Arg.Any<int>());
        }

        [Test]
        public async Task MapAsync_FromCtn_MapFunctionIsCalled()
        {
            var fnMap = Substitute.For<Func<int, Task<bool>>>();
            fnMap(Arg.Any<int>()).Returns(Task.FromResult(true));

            await new Ctn<int>(DefaultValue, None)
                .MapAsync(fnMap);

            await fnMap.Received()(Arg.Any<int>());
        }

        [Test]
        public void Map_FromCtnWithNoStepOutcomes_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<int>(DefaultValue, None);
            var newCtn = ctnWithValueOnly.Map(currentValue => currentValue.ToString());

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue.ToString());
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public async Task MapAsync_FromCtnWithNoStepOutcomes_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<int>(DefaultValue, None);
            var newCtn = await ctnWithValueOnly.MapAsync(async currentValue => currentValue.ToString());

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue.ToString());
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public void Map_FromNull_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<string>(null, None);

            var newValue = 12.45m;
            var newCtn = ctnWithValueOnly.Map(currentValue => newValue);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(newValue);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public async Task MapAsync_FromNull_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<string>(null, None);

            var newValue = 12.45m;
            var newCtn = await ctnWithValueOnly.MapAsync(async currentValue => newValue);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(newValue);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public void Map_ToNull_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<string>("initial value", None);

            const string newValue = null;
            var newCtn = ctnWithValueOnly.Map(currentValue => newValue);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(newValue);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public async Task MapAsync_ToNull_MapsToNewCtnType()
        {
            var ctnWithValueOnly = new Ctn<string>("initial value", None);

            const string newValue = null;
            var newCtn = await ctnWithValueOnly.MapAsync(async currentValue => newValue);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(newValue);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public void Map_FunctionToValueOfSameType_MapsCorrectly()
        {
            var ctnWithValueOnly = new Ctn<int>(DefaultValue, None);
            var newCtn = ctnWithValueOnly.Map(currentValue => currentValue + 1);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue + 1);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public async Task MapAsync_FunctionToValueOfSameType_MapsCorrectly()
        {
            var ctnWithValueOnly = new Ctn<int>(DefaultValue, None);
            var newCtn = await ctnWithValueOnly.MapAsync(async currentValue => currentValue + 1);

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue + 1);
            newCtn.ScenarioTitle.ShouldBeNone();
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(0);
        }

        [Test]
        public void Map_FunctionToValueOfDifferentType_MapsCorrectly()
        {
            var initialCtn = GetInitialCtnWithSuccessfulGiven();
            var newCtn = initialCtn.Map(currentValue => currentValue.ToString());

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue.ToString());
            newCtn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(1);
            newCtn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, GivenStepTitle, Step.Given, 0);
        }

        [Test]
        public async Task MapAsync_FunctionToValueOfDifferentType_MapsCorrectly()
        {
            var initialCtn = GetInitialCtnWithSuccessfulGiven();
            var newCtn = await initialCtn.MapAsync(async currentValue => currentValue.ToString());

            newCtn.Should().NotBeNull();
            newCtn.Content.Should().Be(DefaultValue.ToString());
            newCtn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
            newCtn.StepOutcomes.Should().NotBeNull();
            newCtn.StepOutcomes.Count.Should().Be(1);
            newCtn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, GivenStepTitle, Step.Given, 0);
        }

        [Test]
        public void ToResult_ScenarioTitleNone_TitleIsNullDescriptionIsPrefix()
        {
            var ctn = new Ctn<int>(DefaultValue, None);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEmpty();
            scenarioResult.Title.Should().BeNull();
            scenarioResult.Description.Should().Be("Scenario:");
        }

        [Test]
        public void ToResult_ScenarioTitleSet_TitleIsSetDescriptionIsSet()
        {
            var ctn = new Ctn<int>(DefaultValue, ScenarioTitle);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEmpty();
            scenarioResult.Title.Should().Be(ScenarioTitle);
            scenarioResult.Description.Should().Be($"Scenario: {ScenarioTitle}");
        }

        [Test]
        public void ToResult_StepOutcomesEmpty_StepResultsEmpty()
        {
            var ctn = new Ctn<int>(DefaultValue, None);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEmpty();
        }

        [Test]
        public void ToResult_MultipleStepOutcomesNoScenarioTitle_StepResultsMapped()
        {
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle),
                new StepOutcome(Step.When, Outcome.Inconclusive, None),
                new StepOutcome(Step.Then, Outcome.NotRun, ThenStepTitle)
            };

            var ctn = new Ctn<int>(DefaultValue, stepOutcomes, None);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(3);

            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, $"{GivenStepTitle} [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, null, "When [Inconclusive]", Step.When, 1);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ThenStepTitle, $"{ThenStepTitle} [not run]", Step.Then, 2);
        }

        [Test]
        public void ToResult_MultipleStepOutcomesWithScenarioTitle_StepResultsMapped()
        {
            const string scenarioTitle = "Scenario title";

            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle),
                new StepOutcome(Step.When, Outcome.Inconclusive, None),
                new StepOutcome(Step.Then, Outcome.NotRun, ThenStepTitle)
            };

            var ctn = new Ctn<int>(DefaultValue, stepOutcomes, scenarioTitle);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(3);

            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, $"  {GivenStepTitle} [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, null, "  When [Inconclusive]", Step.When, 1);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ThenStepTitle, $"  {ThenStepTitle} [not run]", Step.Then, 2);
        }

        [Test]
        public void ToResult_MultipleStepOutcomesWithAndButNoScenarioTitle_StepResultsMapped()
        {
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle),
                new StepOutcome(Step.And, Outcome.Pass, AndStepTitle),
                new StepOutcome(Step.When, Outcome.Inconclusive, None),
                new StepOutcome(Step.Then, Outcome.NotRun, ThenStepTitle),
                new StepOutcome(Step.But, Outcome.NotRun, ButStepTitle)
            };

            var ctn = new Ctn<int>(DefaultValue, stepOutcomes, None);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(5);

            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, $"{GivenStepTitle} [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, AndStepTitle, $"  {AndStepTitle} [Passed]", Step.And, 1);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, null, "When [Inconclusive]", Step.When, 2);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ThenStepTitle, $"{ThenStepTitle} [not run]", Step.Then, 3);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ButStepTitle, $"  {ButStepTitle} [not run]", Step.But, 4);
        }

        [Test]
        public void ToResult_MultipleStepOutcomesWithAndButScenarioTitle_StepResultsMapped()
        {
            const string scenarioTitle = "Scenario title";

            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, GivenStepTitle),
                new StepOutcome(Step.And, Outcome.Pass, AndStepTitle),
                new StepOutcome(Step.When, Outcome.Inconclusive, None),
                new StepOutcome(Step.Then, Outcome.NotRun, ThenStepTitle),
                new StepOutcome(Step.But, Outcome.NotRun, ButStepTitle)
            };

            var ctn = new Ctn<int>(DefaultValue, stepOutcomes, scenarioTitle);
            var scenarioResult = ctn.ToResult().Value;

            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Count.Should().Be(5);

            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, GivenStepTitle, $"  {GivenStepTitle} [Passed]", Step.Given, 0);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, AndStepTitle, $"    {AndStepTitle} [Passed]", Step.And, 1);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, null, "  When [Inconclusive]", Step.When, 2);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ThenStepTitle, $"  {ThenStepTitle} [not run]", Step.Then, 3);
            scenarioResult.StepResults.ShouldHaveOutcomeAtIndex(Outcome.NotRun, ButStepTitle, $"    {ButStepTitle} [not run]", Step.But, 4);
        }
    }
}
