namespace BddPipe
{
    internal sealed class StepOutcome
    {
        public Step Step { get; }
        public Outcome Outcome { get; }
        public Option<string> Text { get; }

        public StepOutcome(Step step, Outcome outcome, Option<string> text)
        {
            Step = step;
            Outcome = outcome;
            Text = text;
        }
    }
}
