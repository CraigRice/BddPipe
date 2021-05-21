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

            Action call = () => scenario.GivenRecipe(RecipeStepReturnsDefault<Scenario, int>(StepTitle));

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
        public void GivenRecipe_OfScenarioMapImplementationThrowsException_ShouldBeErrorStepInconclusive()
        {
            var ex = new Exception("test ex");

            var step = Scenario(ScenarioText)
                .GivenRecipe(RecipeStepMapThrowsException<Scenario, int>(StepTitle, ex));

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.NotRun, StepTitle, Step.Given);
            });
        }

        [Test]
        public void GivenRecipe_OfUnitMapImplementationThrowsException_ShouldBeErrorStepInconclusive()
        {
            var ex = new Exception("test ex");

            var step = GivenRecipe(RecipeStepMapThrowsException<Unit, int>(StepTitle, ex));

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.NotRun, StepTitle, Step.Given);
            });
        }

        // next..
        // map arg null
        // map actually maps
        // recipe actually changes pipe state and value

    }
}
