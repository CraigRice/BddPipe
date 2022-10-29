using System;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests.RecipeExtensionsTestHelpers;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    [TestFixture]
    public class ButRecipeTests
    {
        [Test]
        public void ButRecipe_OfRecipeStepFuncTTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int, int> stepFunc = null;

            Action call = () => pipe.ButRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void ButRecipe_OfRecipeStepFuncTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int> stepFunc = null;

            Action call = () => pipe.ButRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void ButRecipe_RecipeStepTTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ButRecipe(RecipeStepReturns<int, int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void ButRecipe_RecipeStepTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ButRecipe(RecipeStepReturns<int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void ButRecipe_RecipeStepTTMapsThenReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .ButRecipe(RecipeStepMapsThenReturns<int, int>(NextStepTitle, NewValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenStepTitle, NextStepTitle, MapThenRecipeResult);
        }
    }
}
