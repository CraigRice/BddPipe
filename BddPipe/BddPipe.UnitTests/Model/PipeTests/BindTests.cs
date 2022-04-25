using System;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.Recipe;
using FluentAssertions;
using static BddPipe.Runner;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model.PipeTests
{
    public static class RecipeBindExtensions
    {
        public static Pipe<T> Decide<T>(this Pipe<T> pipe, Predicate<T> isStepA, RecipeStep<T> recipeStepA, RecipeStep<T> recipeStepB)
        {
            return pipe.Bind(t => isStepA(t)
                ? pipe.AndRecipe(recipeStepA)
                : pipe.AndRecipe(recipeStepB));
        }

        public static Pipe<T> DecideAsync<T>(this Pipe<T> pipe, Predicate<T> isStepA, RecipeStep<T> recipeStepA, RecipeStep<T> recipeStepB)
        {
            return pipe.Bind(async t => isStepA(t)
                ? pipe.AndRecipe(recipeStepA)
                : pipe.AndRecipe(recipeStepB));
        }
    }

    [TestFixture]
    public class BindTests
    {
        private static RecipeStep<int> _stepA = recipe => recipe.Step("step A is called", val => val + 100);
        private static RecipeStep<int> _stepB = recipe => recipe.Step("step B is called", val => val + 1000);

        [Test]
        public void Xa()
        {
            Scenario()
                .Given("an initial value", () => 5)
                .Decide(i => i == 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(105);
                })
                .Run();
        }

        [Test]
        public void Xb()
        {
            Scenario()
                .Given("an initial value", () => 5)
                .Decide(i => i != 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(1005);
                })
                .Run();
        }

        [Test]
        public async Task XaAsync()
        {
            var p = await Scenario()
                .Given("an initial value", () => 5)
                .DecideAsync(i => i == 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(105);
                })
                .RunAsync();
        }
    }
}
