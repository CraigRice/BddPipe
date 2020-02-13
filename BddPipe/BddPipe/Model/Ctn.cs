using System;
using System.Collections.Generic;

namespace BddPipe
{
    /// <summary>
    /// A container to represent progress and state so far
    /// </summary>
    /// <typeparam name="T">Container payload type</typeparam>
    public sealed class Ctn<T>
    {
        internal Option<string> ScenarioTitle { get; }
        internal IReadOnlyList<StepOutcome> StepOutcomes { get; }

        /// <summary>
        /// Container payload instance
        /// </summary>
        public T Content { get; }

        internal Ctn(T content, Option<string> scenarioTitle) : this(content, new StepOutcome[0], scenarioTitle) {}
        internal Ctn(T content, IReadOnlyList<StepOutcome> stepOutcomes, Option<string> scenarioTitle)
        {
            if (stepOutcomes == null) throw new ArgumentNullException(nameof(stepOutcomes));

            StepOutcomes = stepOutcomes;
            Content = content;
            ScenarioTitle = scenarioTitle;
        }
    }

    internal static class CtnExtensions
    {
        /// <summary>
        /// Projects from one value to another and does not impact current step progress.
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="ctn">The <see cref="Ctn{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Ctn{T}"/> instance of the destination type</returns>
        public static Ctn<R> Map<T, R>(this Ctn<T> ctn, Func<T, R> map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));

            return new Ctn<R>(map(ctn.Content), ctn.StepOutcomes, ctn.ScenarioTitle);
        }

        public static Ctn<R> ToCtn<T, R>(this Ctn<T> ctn, R newContent, Some<StepOutcome> withStepOutcome)
        {
            var outcomes = new List<StepOutcome>(ctn.StepOutcomes) { withStepOutcome.Value };
            return new Ctn<R>(newContent, outcomes, ctn.ScenarioTitle);
        }

        public static ScenarioResult ToResult<T>(this Ctn<T> ctn) =>
            new ScenarioResult(
                title: ctn.ScenarioTitle.IfNone(null),
                description: ctn.ScenarioTitle.WithPrefix("Scenario:"),
                stepResults: ctn.StepOutcomes.ToResults(ctn.ScenarioTitle.IsSome)
            );
    }
}
