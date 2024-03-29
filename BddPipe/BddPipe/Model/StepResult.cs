﻿using System.Diagnostics.CodeAnalysis;

namespace BddPipe
{
    /// <summary>
    /// The step result gives a detailed output for each step outcome
    /// </summary>
    public sealed class StepResult
    {
        /// <summary>
        /// The type of this step
        /// </summary>
        public Step Step { get; }

        /// <summary>
        /// The outcome of this step
        /// </summary>
        public Outcome Outcome { get; }

        /// <summary>
        /// Original title for the step
        /// </summary>
        [MaybeNull]
        public string Title { get; }

        /// <summary>
        /// Step description for output
        /// </summary>
        [MaybeNull]
        public string Description { get; }

        /// <summary>
        /// Create a new instance of <see cref="StepResult"/>
        /// </summary>
        public StepResult(Step step, Outcome outcome, [AllowNull] string title, [AllowNull] string description)
        {
            Step = step;
            Outcome = outcome;
            Title = title;
            Description = description;
        }
    }
}
