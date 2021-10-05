using System.Collections.Generic;

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
        public string Title { get; }

        /// <summary>
        /// Scenario description for output
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A sequence of step results that have occurred for this scenario run
        /// </summary>
        public IReadOnlyList<StepResult> StepResults { get; }

        /// <summary>
        /// Create a new instance of <see cref="ScenarioResult"/>
        /// </summary>
        public ScenarioResult(string title, string description, IReadOnlyList<StepResult> stepResults)
        {
            Title = title;
            Description = description;
            StepResults = stepResults;
        }
    }
}
