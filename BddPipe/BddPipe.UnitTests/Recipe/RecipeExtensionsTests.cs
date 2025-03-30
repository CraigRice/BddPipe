using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.Recipe
{
    public sealed class ScenarioInfo(string testValueOne, string? testValueTwo)
    {
        public string TestValueOne { get; } = testValueOne;
        public string? TestValueTwo { get; } = testValueTwo;
    }

    public sealed class ScenarioInfoAlternate(string testValueOne, string? testValueTwo)
    {
        public string TestValueOne { get; } = testValueOne;
        public string? TestValueTwo { get; } = testValueTwo;
    }

    public static class Recipe
    {
        public const string StringArgValueOne = "arg-one";
        public const string StringArgValueTwo = "arg-two";
        public const string GivenStepTitle = "given-step-text";
        public const string AndStepTitle = "and same type is returned as scenario info";
        public const string ButStepTitle = "but same type is returned as scenario info";
        public const string WhenStepTitle = "when same type is returned as scenario info";
        public const string AndStepTitleAlternate = "and a different type is returned as scenario info";
        public const string ButStepTitleAlternate = "but a different type is returned as scenario info";
        public const string ThenStepTitle = "test values are equal";

        public static Func<Pipe<Scenario>, Pipe<ScenarioInfo>> SetupScenarioWithGivenStep() =>
            scenario =>
                scenario.Given(GivenStepTitle, () => new ScenarioInfo(StringArgValueOne, null));


        public static Func<Pipe<Scenario>, Pipe<ScenarioInfo>> SetupScenarioWithGivenStepInErrorState()
        {
            Func<ScenarioInfo> fn = () => throw new ApplicationException("test exception");

            return scenario =>
                scenario.Given(GivenStepTitle, fn);
        }

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> AddToScenarioWithAndStep() =>
            pipe =>
                pipe.And(
                    AndStepTitle,
                    scenarioInfo => new ScenarioInfo(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> AddToScenarioWithButStep() =>
            pipe =>
                pipe.But(
                    ButStepTitle,
                    scenarioInfo => new ScenarioInfo(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> AddToScenarioWithWhenStep() =>
            pipe =>
                pipe.When(
                    WhenStepTitle,
                    scenarioInfo => new ScenarioInfo(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfoAlternate>> AddToScenarioWithAndStepAlternate() =>
            pipe =>
                pipe.And(
                    AndStepTitleAlternate,
                    scenarioInfo => new ScenarioInfoAlternate(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfoAlternate>> AddToScenarioWithButStepAlternate() =>
            pipe =>
                pipe.But(
                    ButStepTitleAlternate,
                    scenarioInfo => new ScenarioInfoAlternate(scenarioInfo.TestValueOne, StringArgValueTwo)
                );

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> AddToScenarioWithAndStepInErrorState(Exception exToThrow)
        {
            Func<ScenarioInfo, ScenarioInfo> fn = _ => throw exToThrow;

            return pipe =>
                pipe.And(
                    AndStepTitle,
                    fn
                );
        }

        public static Func<Pipe<Scenario>, Pipe<ScenarioInfo>> CombinedRecipe() =>
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
        public void Map_ProjectScenarioRecipeValueInSuccessState_MappedValueIsUsed()
        {
            var mapped = Scenario()
                .GivenRecipe(
                    Recipe.SetupScenarioWithGivenStep()
                ).Map(scenarioInfo => scenarioInfo.TestValueOne);

            mapped.ShouldBeSuccessfulGivenStepWithValue(Recipe.GivenStepTitle, Recipe.StringArgValueOne);
        }

        [Test]
        public void Map_ProjectScenarioRecipeChangedTypeValueInSuccessState_MappedValueIsUsed()
        {
            var mapped = Scenario()
                .GivenRecipe(
                    Recipe.SetupScenarioWithGivenStep()
                ).Map(_ => true);

            mapped.ShouldBeSuccessfulGivenStepWithValue(Recipe.GivenStepTitle, true);
        }

        [Test]
        public void Map_ProjectScenarioRecipeValueInErrorState_RemainsInErrorState()
        {
            var mapped = Scenario()
                .GivenRecipe(Recipe.SetupScenarioWithGivenStepInErrorState()
                ).Map(scenarioInfo => scenarioInfo.TestValueTwo);

            mapped.ShouldBeError(ctnError =>
            {
                ctnError.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, Recipe.GivenStepTitle, Step.Given);
            });
        }

        [Test]
        public void Map_ProjectScenarioRecipeValueInErrorState_MapFunctionIsNotCalled()
        {
            var fnMap = Substitute.For<Func<ScenarioInfo, int>>();
            fnMap(Arg.Any<ScenarioInfo>()).Returns(76);

            Scenario()
                .GivenRecipe(
                    Recipe.SetupScenarioWithGivenStepInErrorState()
                ).Map(fnMap);

            fnMap.DidNotReceive()(Arg.Any<ScenarioInfo>());
        }

        [Test]
        public void Map_ProjectScenarioRecipeValueInSuccessState_MapFunctionIsCalled()
        {
            var fnMap = Substitute.For<Func<ScenarioInfo, int>>();
            fnMap(Arg.Any<ScenarioInfo>()).Returns(76);

            Scenario()
                .GivenRecipe(
                    Recipe.SetupScenarioWithGivenStep()
                ).Map(fnMap);

            fnMap.Received()(Arg.Any<ScenarioInfo>());
        }

        [Test]
        public void Map_ProjectPipeRecipeChangedTypeValueInSuccessState_MappedValueIsUsed()
        {
            var mapped = Scenario()
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStep())
                .Map(_ => true);

            mapped.ShouldBeSuccessfulSecondStepWithValue(Step.And, Recipe.GivenStepTitle, Recipe.AndStepTitle, true);
        }

        [Test]
        public void Map_ProjectPipeRecipeValueInErrorState_RecipeStepInErrorState()
        {
            var exToThrow = new ApplicationException("test exception message");

            var mapped = Scenario()
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStepInErrorState(exToThrow)
                ).Map(scenarioInfo => scenarioInfo.TestValueOne);

            mapped.ShouldBeErrorSecondStepWithException(Step.And, Recipe.GivenStepTitle, Recipe.AndStepTitle, exToThrow);
        }

        [Test]
        public void Map_ProjectPipeRecipeValueInErrorState_MapFunctionIsNotCalled()
        {
            var exToThrow = new ApplicationException("test exception message");
            var fnMap = Substitute.For<Func<ScenarioInfo, int>>();
            fnMap(Arg.Any<ScenarioInfo>()).Returns(76);

            Scenario()
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStepInErrorState(exToThrow)
                ).Map(fnMap);

            fnMap.DidNotReceive()(Arg.Any<ScenarioInfo>());
        }

        [Test]
        public void Map_ProjectPipeRecipeValueInSuccessState_MapFunctionIsCalled()
        {
            var fnMap = Substitute.For<Func<ScenarioInfo, int>>();
            fnMap(Arg.Any<ScenarioInfo>()).Returns(76);

            Scenario()
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .AndRecipe(Recipe.AddToScenarioWithAndStep()
                ).Map(fnMap);

            fnMap.Received()(Arg.Any<ScenarioInfo>());
        }

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
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, Recipe.GivenStepTitle, Step.Given);
            });
        }

        [Test]
        public void GivenRecipe_RecipeFunctionNull_ThrowArgNullException()
        {
            var scenario = Scenario();

            Func<Pipe<Scenario>, Pipe<int>> fn = null!;

            Action call = () => scenario.GivenRecipe(fn);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeFunction");
        }

        [Test]
        public void AndRecipe_RecipeFunctionNull_ThrowArgNullException()
        {
            var pipe = Given("anything", () => 4);

            Func<Pipe<int>, Pipe<int>> fn = null!;

            Action call = () => pipe.AndRecipe(fn);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeFunction");
        }

        [Test]
        public void ButRecipe_RecipeFunctionNull_ThrowArgNullException()
        {
            var pipe = Given("anything", () => 4);

            Func<Pipe<int>, Pipe<int>> fn = null!;

            Action call = () => pipe.ButRecipe(fn);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeFunction");
        }

        [Test]
        public void ThenRecipe_RecipeFunctionNull_ThrowArgNullException()
        {
            var pipe = Given("anything", () => 4);

            Func<Pipe<int>, Pipe<int>> fn = null!;

            Action call = () => pipe.ThenRecipe(fn);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeFunction");
        }

        [Test]
        public void WhenRecipe_RecipeFunctionNull_ThrowArgNullException()
        {
            var pipe = Given("anything", () => 4);

            Func<Pipe<int>, Pipe<int>> fn = null!;

            Action call = () => pipe.WhenRecipe(fn);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeFunction");
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
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitle, Step.And, 1);
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
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitleAlternate, Step.And, 1);
            });
        }

        [Test]
        public void ButRecipe_AddToScenarioWithButStep_AppliesAndStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .ButRecipe(Recipe.AddToScenarioWithButStep());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.ButStepTitle, Step.But, 1);
            });
        }

        [Test]
        public void ButRecipe_AddToScenarioWithButStepAlternate_AppliesAndStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .ButRecipe(Recipe.AddToScenarioWithButStepAlternate());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.ButStepTitleAlternate, Step.But, 1);
            });
        }

        [Test]
        public void WhenRecipe_AddToScenarioWithWhenStep_AppliesAndStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupScenarioWithGivenStep())
                .WhenRecipe(Recipe.AddToScenarioWithWhenStep());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValueOne.Should().Be(Recipe.StringArgValueOne);
                ctn.Content.TestValueTwo.Should().Be(Recipe.StringArgValueTwo);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.Count.Should().Be(2);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.WhenStepTitle, Step.When, 1);
            });
        }

        [Test]
        public void AndRecipe_CombinedRecipeOfGivenWithAnd_AppliesSteps()
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
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.GivenStepTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.AndStepTitle, Step.And, 1);
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
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, Recipe.ThenStepTitle, Step.Then, 1);
            });
        }
    }
}
