using System;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Recipe.RecipeExtensions;
using static BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests.RecipeExtensionsTestHelpers;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    [TestFixture]
    public class ThenRecipeTests
    {
        [Test]
        public void ThenRecipe_OfRecipeStepFuncTTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int, int> stepFunc = null;

            Action call = () => pipe.ThenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void ThenRecipe_OfRecipeStepFuncTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int> stepFunc = null;

            Action call = () => pipe.ThenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void ThenRecipe_RecipeStepTTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ThenRecipe(RecipeStepReturns<int, int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.Then, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void ThenRecipe_RecipeStepTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ThenRecipe(RecipeStepReturns<int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.Then, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void ThenRecipe_RecipeStepTTMapsThenReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ThenRecipe(RecipeStepMapsThenReturns<int, int>(NextStepTitle, NewValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.Then, GivenStepTitle, NextStepTitle, MapThenRecipeResult);
        }
    }
}
