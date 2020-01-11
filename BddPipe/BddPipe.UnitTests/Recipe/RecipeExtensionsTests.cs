using System;
using BddPipe.Model;
using BddPipe.Recipe;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.Recipe
{
    public sealed class ScenarioInfo
    {
        public string TestValueOne { get; }
        public string TestValueTwo { get; }

        public ScenarioInfo(string testValueOne, string testValueTwo)
        {
            TestValueOne = testValueOne;
            TestValueTwo = testValueTwo;
        }
    }

    public sealed class ScenarioInfoAlternate
    {
        public string TestValueOne { get; }
        public string TestValueTwo { get; }

        public ScenarioInfoAlternate(string testValueOne, string testValueTwo)
        {
            TestValueOne = testValueOne;
            TestValueTwo = testValueTwo;
        }
    }

    public static class Recipe
    {
        public const string StringArgValueOne = "arg-one";
        public const string StringArgValueTwo = "arg-two";
        public const string GivenStepTitle = "given-step-text";
        public const string AndStepTitle = "the same type is returned as scenario info";
        public const string AndStepTitleAlternate = "a different type is returned as scenario info";
        public const string ThenStepTitle = "test values are equal";

        public static Func<Scenario, Pipe<ScenarioInfo>> SetupScenarioWithGivenStep() =>
            scenario =>
                scenario.Given(GivenStepTitle, () => new ScenarioInfo(StringArgValueOne, null));

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> AddToScenarioWithAndStep() =>
            pipe =>
                pipe.And(
                    AndStepTitle,
                    scenarioInfo => new ScenarioInfo(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfoAlternate>> AddToScenarioWithAndStepAlternate() =>
            pipe =>
                pipe.And(
                    AndStepTitleAlternate,
                    scenarioInfo => new ScenarioInfoAlternate(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Scenario, Pipe<ScenarioInfo>> CombinedRecipe() =>
            scenario =>
                scenario
                    .GivenRecipe(SetupScenarioWithGivenStep())
                    .AndRecipe(AddToScenarioWithAndStep());

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> ThenAssertRecipe() =>
            pipe =>
                pipe.Then(
                    ThenStepTitle,
                    scenarioInfo => { scenarioInfo.TestValueOne.Should().Be(scenarioInfo.TestValueTwo); });
    }

    [TestFixture]
    public class RecipeExtensionsTests
    {
        private const string ScenarioText = "scenario-text";

        [Test]
        public void GivenRecipe_SetupScenarioWithGivenStep_AppliesGivenStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(null);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleOutcome(Outcome.Pass, Recipe.GivenStepTitle, Step.Given);
            });
        }

        [Test]
        public void AndRecipe_AddToScenarioWithAndStep_AppliesAndStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStep());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitle, Step.And, 1);
            });
        }

        [Test]
        public void AndRecipe_AddToScenarioWithAndStepAlternate_AppliesAndStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStepAlternate());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitleAlternate, Step.And, 1);
            });
        }

        [Test]
        public void AndRecipe_CombinedRecipe_AppliesSteps()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.CombinedRecipe());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitle, Step.And, 1);
            });
        }

        [Test]
        public void ThenRecipe_AppliedToAssertFollowingInitialSteps_RunsRecipe()
        {
            const string givenTitle = "a scenario info of the same string values";

            var scenarioSetup = Scenario(ScenarioText)
                .Given(givenTitle,
                    () => new ScenarioInfo(Recipe.StringArgValueOne, Recipe.StringArgValueOne))
                .ThenRecipe(Recipe.ThenAssertRecipe());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueOne);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, Recipe.ThenStepTitle, Step.Then, 1);
            });
        }
    }
}
