using BddPipe.Model;
using System;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    public static class RecipeExtensionsTestHelpers
    {
        public const string GivenStepTitle = "a given step title";
        public const string NextStepTitle = "with the next step";
        public const string ScenarioText = "ScenarioText";
        public const int NewValue = 3434;
        public const int NextValue = 987;
        public const string MapToStringResult = "map result";
        public const string MapThenRecipeResult = "map result then 3434";

        public static Pipe<int> GetPipeAfterGiven() =>
            Scenario(ScenarioText)
                .Given(GivenStepTitle, _ => NewValue);

        public static RecipeStep<TSource, T> RecipeStepMapThrowsException<TSource, T>(
            string title,
            Exception exceptionToThrow)
            where T : struct
        {
            return recipe => recipe
                .Map<T>(_ => throw exceptionToThrow)
                .Step(title, _ => default(T));
        }

        public static RecipeStep<TSource, T> RecipeStepMapArgumentNull<TSource, T>()
            where T : struct
        {
            Func<TSource, T> fn = null!;

            return recipe => recipe
                .Map(fn)
                .Step(GivenStepTitle, _ => default(T));
        }

        public static RecipeStep<TSource, T> RecipeStepStepArgumentNull<TSource, T>()
        {
            Func<TSource, T> fn = null!;

            return recipe => recipe
                .Step(GivenStepTitle, fn);
        }

        public static RecipeStep<TSource, T> RecipeStepReturns<TSource, T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Step(title, _ => returnValue);
        }

        public static RecipeStep<T> RecipeStepReturns<T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Step(title, _ => returnValue);
        }

        public static RecipeStep<TSource, string> RecipeStepMapsThenReturns<TSource, T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Map(_ => MapToStringResult)
                .Step(title, mapResult => $"{mapResult} then {returnValue}");
        }
    }
}
