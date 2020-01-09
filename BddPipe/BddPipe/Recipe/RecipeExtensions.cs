using System;
using BddPipe.Model;

namespace BddPipe.Recipe
{
    /// <summary>
    /// Allows reusable recipes to be defined and plugged into the pipeline as a step or combination of steps.
    /// </summary>
    public static class RecipeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pipe"></param>
        /// <param name="recipeFunction"></param>
        /// <returns></returns>
        public static Pipe<T> GivenRecipe<T>(
            this Scenario pipe,
            Func<
                Scenario,
                Pipe<T>
            > recipeFunction) => recipeFunction(pipe);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pipe"></param>
        /// <param name="recipeFunction"></param>
        /// <returns></returns>
        public static Pipe<R> AndRecipe<T, R>(
            this Pipe<T> pipe,
            Func<
                Pipe<T>,
                Pipe<R>
            > recipeFunction) => recipeFunction(pipe);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pipe"></param>
        /// <param name="recipeFunction"></param>
        /// <returns></returns>
        public static Pipe<R> ThenRecipe<T, R>(
            this Pipe<T> pipe,
            Func<
                Pipe<T>,
                Pipe<R>
            > recipeFunction) => recipeFunction(pipe);

    }
}
