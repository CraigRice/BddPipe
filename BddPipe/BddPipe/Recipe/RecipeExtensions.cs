using System;
using BddPipe.Model;

namespace BddPipe.Recipe
{
    /// <summary>
    /// Allows reusable recipes to be defined and plugged into the pipeline as a step or combination of steps.
    /// </summary>
    public static partial class RecipeExtensions
    {
        /// <summary>
        /// Allow a function to run in place of the 'Given' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="scenario">Current <see cref="Scenario"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Scenario"/></param>
        /// <returns>A <see cref="Pipe{R}"/> with recipe steps applied via the supplied function.</returns>
        public static Pipe<R> GivenRecipe<R>(this Scenario scenario, Func<Scenario, Pipe<R>> recipeFunction)
        {
            if (scenario == null) { throw new ArgumentNullException(nameof(scenario)); }
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(scenario);
        }

        /// <summary>
        /// Allow a function to run in place of the 'And' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="T">Initial incoming type represented by the <see cref="Pipe{T}"/></typeparam>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="pipe">Current <see cref="Pipe{T}"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Pipe{T}"/></param>
        /// <returns>A <see cref="Pipe{R}"/> with recipe steps applied via the supplied function.</returns>
        public static Pipe<R> AndRecipe<T, R>(this Pipe<T> pipe,  Func<Pipe<T>, Pipe<R>> recipeFunction)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(pipe);
        }

        /// <summary>
        /// Allow a function to run in place of the 'Then' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="T">Initial incoming type represented by the <see cref="Pipe{T}"/></typeparam>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="pipe">Current <see cref="Pipe{T}"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Pipe{T}"/></param>
        /// <returns>A <see cref="Pipe{R}"/> instance as a result of the recipe function.</returns>
        public static Pipe<R> ThenRecipe<T, R>(this Pipe<T> pipe, Func<Pipe<T>, Pipe<R>> recipeFunction)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(pipe);
        }

        /// <summary>
        /// Allow a function to run in place of the 'But' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="T">Initial incoming type represented by the <see cref="Pipe{T}"/></typeparam>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="pipe">Current <see cref="Pipe{T}"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Pipe{T}"/></param>
        /// <returns>A <see cref="Pipe{R}"/> with recipe steps applied via the supplied function.</returns>
        public static Pipe<R> ButRecipe<T, R>(this Pipe<T> pipe, Func<Pipe<T>, Pipe<R>> recipeFunction)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(pipe);
        }

        /// <summary>
        /// Projects the value represented by the recipe function result to a new value.
        /// </summary>
        /// <typeparam name="T">Recipe output type</typeparam>
        /// <typeparam name="R">Map output type</typeparam>
        /// <param name="recipeFunction">A recipe function based on <see cref="Scenario"/> input to perform the map on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A <see cref="Pipe{R}"/> instance representing the mapped type, and containing the mapped value if in a successful state.</returns>
        public static Func<Scenario, Pipe<R>> Map<T, R>(this Func<Scenario, Pipe<T>> recipeFunction, Func<T, R> map)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }
            if (map == null) { throw new ArgumentNullException(nameof(map)); }

            return scenario => recipeFunction(scenario).Map(map);
        }

        /// <summary>
        /// Projects the value represented by the recipe function result to a new value.
        /// </summary>
        /// <typeparam name="T1">Recipe input type</typeparam>
        /// <typeparam name="T2">Recipe output type</typeparam>
        /// <typeparam name="R">Map output type</typeparam>
        /// <param name="recipeFunction">A recipe function based on <see cref="Pipe{T1}"/> input to perform the map on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A <see cref="Pipe{R}"/> instance representing the mapped type, and containing the mapped value if in a successful state.</returns>
        public static Func<Pipe<T1>, Pipe<R>> Map<T1, T2, R>(this Func<Pipe<T1>, Pipe<T2>> recipeFunction, Func<T2, R> map)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }
            if (map == null) { throw new ArgumentNullException(nameof(map)); }

            return pipe => recipeFunction(pipe).Map(map);
        }
    }
}
