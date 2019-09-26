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
        public string Title { get; }

        /// <summary>
        /// Step description for output
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Create a new instance of <see cref="StepResult"/>
        /// </summary>
        public StepResult(Step step, Outcome outcome, string title, string description)
        {
            Step = step;
            Outcome = outcome;
            Title = title;
            Description = description;
        }
    }
}
