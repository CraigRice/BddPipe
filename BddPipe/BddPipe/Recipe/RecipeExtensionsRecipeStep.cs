using System;
using System.Diagnostics.CodeAnalysis;
using BddPipe.Model;

namespace BddPipe
{
    /// <summary>
    /// Allows reusable recipes to be defined and plugged into the pipeline as a step or combination of steps.
    /// </summary>
    public static partial class RecipeExtensions
    {
        /// <summary>
        /// <see cref="Step.Given"/> Recipe
        /// </summary>
        public static Pipe<R> GivenRecipe<R>([DisallowNull] RecipeStep<Unit, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            var pipe = Runner.CreatePipe();

            return recipeStepFunc(new Recipe<Unit, R>(pipe, Step.Given));
        }

        /// <summary>
        /// <see cref="Step.Given"/> Recipe
        /// </summary>
        public static Pipe<R> GivenRecipe<R>(this Pipe<Scenario> scenario, [DisallowNull] RecipeStep<Scenario, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<Scenario, R>(scenario, Step.Given));
        }

        /// <summary>
        /// <see cref="Step.And"/> Recipe
        /// </summary>
        public static Pipe<R> AndRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.And));
        }

        /// <summary>
        /// <see cref="Step.And"/> Recipe
        /// </summary>
        public static Pipe<T> AndRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.And));
        }

        /// <summary>
        /// <see cref="Step.But"/> Recipe
        /// </summary>
        public static Pipe<R> ButRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.But));
        }

        /// <summary>
        /// <see cref="Step.But"/> Recipe
        /// </summary>
        public static Pipe<T> ButRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.But));
        }

        /// <summary>
        /// <see cref="Step.When"/> Recipe
        /// </summary>
        public static Pipe<R> WhenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.When));
        }

        /// <summary>
        /// <see cref="Step.When"/> Recipe
        /// </summary>
        public static Pipe<T> WhenRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.When));
        }

        /// <summary>
        /// <see cref="Step.Then"/> Recipe
        /// </summary>
        public static Pipe<R> ThenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T, R> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, R>(pipe, Step.Then));
        }

        /// <summary>
        /// <see cref="Step.Then"/> Recipe
        /// </summary>
        public static Pipe<T> ThenRecipe<T>(this Pipe<T> pipe, [DisallowNull] RecipeStep<T> recipeStepFunc)
        {
            if (recipeStepFunc == null) { throw new ArgumentNullException(nameof(recipeStepFunc)); }

            return recipeStepFunc(new Recipe<T, T>(pipe, Step.Then));
        }
    }
}
