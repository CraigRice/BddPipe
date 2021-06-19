using System;
using System.Collections.Generic;
using System.Text;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Recipe.RecipeExtensions;
using static BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests.RecipeExtensionsTestHelpers;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    [TestFixture]
    public class WhenRecipeTests
    {
        [Test]
        public void WhenRecipe_OfRecipeStepFuncTTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int, int> stepFunc = null;

            Action call = () => pipe.WhenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void WhenRecipe_OfRecipeStepFuncTNull_ThrowsArgNullException()
        {
            var pipe = GetPipeAfterGiven();

            RecipeStep<int> stepFunc = null;

            Action call = () => pipe.WhenRecipe(stepFunc);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("recipeStepFunc");
        }

        [Test]
        public void WhenRecipe_RecipeStepTTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .WhenRecipe(RecipeStepReturns<int, int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.When, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void WhenRecipe_RecipeStepTReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .WhenRecipe(RecipeStepReturns<int>(NextStepTitle, NextValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.When, GivenStepTitle, NextStepTitle, NextValue);
        }

        [Test]
        public void WhenRecipe_RecipeStepTTMapsThenReturnsValue_ShouldBeSuccessWithNewValue()
        {
            var step = GetPipeAfterGiven()
                .WhenRecipe(RecipeStepMapsThenReturns<int, int>(NextStepTitle, NewValue));
            step.ShouldBeSuccessfulSecondStepWithValue(Step.When, GivenStepTitle, NextStepTitle, MapThenRecipeResult);
        }
    }
}
