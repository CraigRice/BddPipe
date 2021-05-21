using System;
using BddPipe.Model;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    public static class RecipeExtensionsTestHelpers
    {
        public const string StepTitle = "a step title";
        public const string ScenarioText = "ScenarioText";

        public static Exception GetTestException() =>
            new ApplicationException("test exception message");

        public static RecipeStep<TSource, T> RecipeStepMapThrowsException<TSource, T>(
            string title,
            Exception exceptionToThrow
            )
        {
            return recipe => recipe
                .Map<T>(tSource => throw exceptionToThrow)
                .Step(title, t => default(T));
        }

        public static RecipeStep<TSource, T> RecipeStepReturnsDefault<TSource, T>(
            string title
        )
        {
            return recipe => recipe
                .Step(title, tSource => default(T));
        }
    }
}
