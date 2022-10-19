using System;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests.RecipeExtensionsTestHelpers;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    [TestFixture]
    public class AndRecipeTests
    {
        [Test]
        public void AndRecipe_RecipeStepFuncTTIsNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int, int> stepFunc = null;

            Action call = () => pipe.AndRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void AndRecipe_RecipeStepFuncTIsNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int> stepFunc = null;

            Action call = () => pipe.AndRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void AndRecipe_RecipeStepTTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .AndRecipe(RecipeStepReturns<int, int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void AndRecipe_RecipeStepTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .AndRecipe(RecipeStepReturns<int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void AndRecipe_RecipeStepTTMapsThenReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .AndRecipe(RecipeStepMapsThenReturns<int, int>(NextStepTitle, NewValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenStepTitle, NextStepTitle, MapThenRecipeResult);
        }
    }
}
