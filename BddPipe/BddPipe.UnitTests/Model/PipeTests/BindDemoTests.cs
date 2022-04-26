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
        public static Pipe<T> OptionallyRunStep<T>(this Pipe<T> pipe, Predicate<T> isStepA, RecipeStep<T> recipeStepA, RecipeStep<T> recipeStepB)
        {
            return pipe.Bind(t => isStepA(t)
                ? pipe.AndRecipe(recipeStepA)
                : pipe.AndRecipe(recipeStepB));
        }

        public static Pipe<T> OptionallyRunStepAsync<T>(this Pipe<T> pipe, Predicate<T> isStepA, RecipeStep<T> recipeStepA, RecipeStep<T> recipeStepB)
        {
            return pipe.Bind(t => isStepA(t)
                ? Task.FromResult(pipe.AndRecipe(recipeStepA))
                : Task.FromResult(pipe.AndRecipe(recipeStepB)));
        }
    }

    [TestFixture]
    public partial class BindTests
    {
        private const int stepAResult = 1324;
        private const int stepBResult = 7876;
        private static RecipeStep<int> _stepA = recipe => recipe.Step("step A is called", val => stepAResult);
        private static RecipeStep<int> _stepB = recipe => recipe.Step("step B is called", val => stepBResult);

        [Test]
        public void Bind_OptionallyRunStepDemoStepA_RunsStepA() =>
            Scenario()
                .Given("an initial value", () => 5)
                .OptionallyRunStep(i => i == 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(stepAResult);
                })
                .Run();

        [Test]
        public void Bind_OptionallyRunStepDemoStepB_RunsStepB() =>
            Scenario()
                .Given("an initial value", () => 5)
                .OptionallyRunStep(i => i != 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(stepBResult);
                })
                .Run();

        [Test]
        public Task Bind_OptionallyRunStepAsyncDemoStepA_RunsStepA() =>
            Scenario()
                .Given("an initial value", () => 5)
                .OptionallyRunStepAsync(i => i == 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(stepAResult);
                })
                .RunAsync();

        [Test]
        public Task Bind_OptionallyRunStepAsyncDemoStepB_RunsStepB() =>
            Scenario()
                .Given("an initial value", () => 5)
                .OptionallyRunStepAsync(i => i != 5, _stepA, _stepB)
                .Then("value is correct", result =>
                {
                    result.Should().Be(stepBResult);
                })
                .RunAsync();
    }
}
