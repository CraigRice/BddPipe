using System;
using BddPipe.Model;

namespace BddPipe.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RecipeExtensions
    {
        public static Pipe<T> GivenRecipe<T>(
            this Scenario pipe,
            Func<
                Scenario,
                Pipe<T>
            > recipeFunction) => recipeFunction(pipe);

        public static Pipe<T> AndRecipe<T>(
            this Pipe<T> pipe,
            Func<
                Pipe<T>,
                Pipe<T>
            > recipeFunction) => recipeFunction(pipe);

        public static Pipe<R> AndRecipe<T, R>(
            this Pipe<T> pipe,
            Func<
                Pipe<T>,
                Pipe<R>
            > recipeFunction) => recipeFunction(pipe);
    }
}
