using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BddPipe.Model
{
    /// <summary>
    /// A container to represent progress and state so far
    /// </summary>
    /// <typeparam name="T">Container payload type</typeparam>
    internal sealed class Ctn<T>
    {
        public Option<string> ScenarioTitle { get; }
        public IReadOnlyList<StepOutcome> StepOutcomes { get; }

        /// <summary>
        /// Container payload instance
        /// </summary>
        [MaybeNull]
        public T Content { get; }

        public Ctn(T content, in Option<string> scenarioTitle) : this(content, Array.Empty<StepOutcome>(), scenarioTitle) {}
        public Ctn(T content, IReadOnlyList<StepOutcome> stepOutcomes, Option<string> scenarioTitle)
        {
            StepOutcomes = stepOutcomes ?? throw new ArgumentNullException(nameof(stepOutcomes));
            Content = content;
            ScenarioTitle = scenarioTitle;
        }
    }
}
