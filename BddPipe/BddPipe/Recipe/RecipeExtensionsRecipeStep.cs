using System;
using System.Diagnostics.CodeAnalysis;
using BddPipe.Model;

namespace BddPipe.Recipe
{
    /// <summary>
    /// Allows reusable recipes to be defined and plugged into the pipeline as a step or combination of steps.
    /// </summary>
    public static partial class RecipeExtensions
    {
        public static Pipe<R> GivenRecipe<R>([DisallowNull] RecipeStep<Unit, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            var pipe = Runner.CreatePipe();

            return recipeStepFunc(new Recipe<Unit, R>(pipe, Step.Given));
        }

        public static Pipe<R> GivenRecipe<R>(this Pipe<Scenario> scenario, [DisallowNull] RecipeStep<Scenario, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<Scenario, R>(scenario, Step.Given));
        }

        public static Pipe<R> AndRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.And));
        }

        public static Pipe<T> AndRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.And));
        }

        public static Pipe<R> ButRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.But));
        }

        public static Pipe<T> ButRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.But));
        }

        public static Pipe<R> ThenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.Then));
        }

        public static Pipe<T> ThenRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.Then));
        }

        public static Pipe<R> WhenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.When));
        }

        public static Pipe<T> WhenRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.When));
        }
    }
}
