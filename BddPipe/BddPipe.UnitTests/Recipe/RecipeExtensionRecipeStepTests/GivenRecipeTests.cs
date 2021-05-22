using System;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Recipe.RecipeExtensions;
using static BddPipe.Runner;
using static BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests.RecipeExtensionsTestHelpers;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    [TestFixture]
    public class GivenRecipeTests
    {
        [Test]
        public void GivenRecipe_OfScenarioRecipeStepFuncNull_ThrowsArgNullException()
        {
            RecipeStep<Scenario, int> stepFunc = null;

            Action call = () => Scenario().GivenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void GivenRecipe_OfScenarioScenarioNull_ThrowsArgNullException()
        {
            RecipeStep<Scenario, int> stepFunc = null;
            Scenario scenario = null;

            Action call = () => scenario.GivenRecipe(RecipeStepReturnsDefault<Scenario, int>(GivenStepTitle));

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenRecipe_OfUnitRecipeStepFuncNull_ThrowsArgNullException()
        {
            RecipeStep<Unit, int> stepFunc = null;

            Action call = () => GivenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void GivenRecipe_OfScenarioMapArgumentNull_ThrowsArgNullException()
        {
            Action call = () => Scenario().GivenRecipe(RecipeStepMapArgumentNull<Scenario, int>());

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void GivenRecipe_OfUnitMapArgumentNull_ThrowsArgNullException()
        {
            Action call = () => GivenRecipe(RecipeStepMapArgumentNull<Unit, int>());

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void GivenRecipe_OfScenarioStepArgumentNull_ThrowsArgNullException()
        {
            Action call = () => Scenario().GivenRecipe(RecipeStepStepArgumentNull<Scenario, int>());

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenRecipe_OfUnitStepArgumentNull_ThrowsArgNullException()
        {
            Action call = () => GivenRecipe(RecipeStepStepArgumentNull<Unit, int>());

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenRecipe_OfScenarioMapImplementationThrowsException_ShouldBeErrorStepInconclusive()
        {
            var ex = new Exception("test ex");

            var step = Scenario(ScenarioText)
                .GivenRecipe(RecipeStepMapThrowsException<Scenario, int>(GivenStepTitle, ex));

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.NotRun, GivenStepTitle, Step.Given);
            });
        }

        [Test]
        public void GivenRecipe_OfUnitMapImplementationThrowsException_ShouldBeErrorStepInconclusive()
        {
            var ex = new Exception("test ex");

            var step = GivenRecipe(RecipeStepMapThrowsException<Unit, int>(GivenStepTitle, ex));

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.NotRun, GivenStepTitle, Step.Given);
            });
        }

        [Test]
        public void GivenRecipe_OfScenarioRecipeStepReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = Scenario().GivenRecipe(RecipeStepReturns<Scenario, int>(GivenStepTitle, NewValue));
            step.ShouldBeSuccessfulGivenStepWithValue(GivenStepTitle, NewValue);
        }

        [Test]
        public void GivenRecipe_OfUnitRecipeStepReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GivenRecipe(RecipeStepReturns<Unit, int>(GivenStepTitle, NewValue));
            step.ShouldBeSuccessfulGivenStepWithValue(GivenStepTitle, NewValue);
        }

        [Test]
        public void GivenRecipe_OfScenarioMapThenRecipeStepReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = Scenario().GivenRecipe(RecipeStepMapsThenReturns<Scenario, int>(GivenStepTitle, NewValue));
            step.ShouldBeSuccessfulGivenStepWithValue(GivenStepTitle, MapThenRecipeResult);
        }

        [Test]
        public void GivenRecipe_OfUnitMapThenRecipeStepReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GivenRecipe(RecipeStepMapsThenReturns<Unit, int>(GivenStepTitle, NewValue));
            step.ShouldBeSuccessfulGivenStepWithValue(GivenStepTitle, MapThenRecipeResult);
        }
    }
}
