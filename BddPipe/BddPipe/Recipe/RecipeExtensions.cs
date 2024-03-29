﻿using System;
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
        /// Allow a function to run in place of the 'Given' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="scenario">Current <see cref="Scenario"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Scenario"/></param>
        /// <returns>A <see cref="Pipe{R}"/> with recipe steps applied via the supplied function.</returns>
        public static Pipe<R> GivenRecipe<R>(this Pipe<Scenario> scenario, [DisallowNull] Func<Pipe<Scenario>, Pipe<R>> recipeFunction)
        {
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
        public static Pipe<R> AndRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] Func<Pipe<T>, Pipe<R>> recipeFunction)
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
        public static Pipe<R> ButRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] Func<Pipe<T>, Pipe<R>> recipeFunction)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(pipe);
        }

        /// <summary>
        /// Allow a function to run in place of the 'When' step to add a reusable step or series of steps.
        /// </summary>
        /// <typeparam name="T">Initial incoming type represented by the <see cref="Pipe{T}"/></typeparam>
        /// <typeparam name="R">Type to be represented by the <see cref="Pipe{R}"/> after the step(s) are run.</typeparam>
        /// <param name="pipe">Current <see cref="Pipe{T}"/> instance to add step(s) to.</param>
        /// <param name="recipeFunction">Function describing the step(s) added to the <see cref="Pipe{T}"/></param>
        /// <returns>A <see cref="Pipe{R}"/> instance as a result of the recipe function.</returns>
        public static Pipe<R> WhenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] Func<Pipe<T>, Pipe<R>> recipeFunction)
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
        public static Pipe<R> ThenRecipe<T, R>(this Pipe<T> pipe, [DisallowNull] Func<Pipe<T>, Pipe<R>> recipeFunction)
        {
            if (recipeFunction == null) { throw new ArgumentNullException(nameof(recipeFunction)); }

            return recipeFunction(pipe);
        }
    }
}
