using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BddPipe
{
    /// <summary>
    /// The scenario result gives a detailed output for each step outcome
    /// </summary>
    public sealed class ScenarioResult
    {
        /// <summary>
        /// Original scenario title that was supplied
        /// </summary>
        [MaybeNull]
        public string Title { get; }

        /// <summary>
        /// Scenario description for output
        /// </summary>
        [MaybeNull]
        public string Description { get; }

        /// <summary>
        /// A sequence of step results that have occurred for this scenario run
        /// </summary>
        [NotNull]
        public IReadOnlyList<StepResult> StepResults { get; }

        /// <summary>
        /// Create a new instance of <see cref="ScenarioResult"/>
        /// </summary>
        public ScenarioResult([AllowNull] string title, [AllowNull] string description, [DisallowNull] IReadOnlyList<StepResult> stepResults)
        {
            StepResults = stepResults ?? throw new ArgumentNullException(nameof(stepResults));
            Title = title;
            Description = description;
        }
    }
}
