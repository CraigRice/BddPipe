﻿using System;
using BddPipe.Model;

namespace BddPipe.UnitTests.Recipe.RecipeExtensionRecipeStepTests
{
    public static class RecipeExtensionsTestHelpers
    {
        public const string StepTitle = "a step title";
        public const string ScenarioText = "ScenarioText";
        public const int NewValue = 3434;
        public const string MapToStringResult = "map result";
        public const string MapThenRecipeResult = "map result then 3434";

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

        public static RecipeStep<TSource, T> RecipeStepMapArgumentNull<TSource, T>()
        {
            return recipe => recipe
                .Map<T>(null)
                .Step(StepTitle, t => default(T));
        }

        public static RecipeStep<TSource, T> RecipeStepStepArgumentNull<TSource, T>()
        {
            return recipe => recipe
                .Step(StepTitle, (Func<TSource, T>)null);
        }

        public static RecipeStep<TSource, T> RecipeStepReturnsDefault<TSource, T>(
            string title
        )
        {
            return recipe => recipe
                .Step(title, tSource => default(T));
        }

        public static RecipeStep<TSource, T> RecipeStepReturns<TSource, T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Step(title, tSource => returnValue);
        }

        public static RecipeStep<T> RecipeStepReturns<T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Step(title, tSource => returnValue);
        }

        public static RecipeStep<TSource, string> RecipeStepMapsThenReturns<TSource, T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Map(source => MapToStringResult)
                .Step(title, mapResult => $"{mapResult} then {returnValue}");
        }

        public static RecipeStep<string> RecipeStepMapsThenReturns<T>(
            string title,
            T returnValue
        )
        {
            return recipe => recipe
                .Map(source => MapToStringResult)
                .Step(title, mapResult => $"{mapResult} then {returnValue}");
        }
    }
}
