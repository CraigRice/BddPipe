using System;
using System.Collections.Generic;
using System.Linq;
using BddPipe.Model;

namespace BddPipe
{
    internal static class StepResultExtensions
    {
        private const int DefaultIndentSize = 2;

        private static Some<string> ToOutcomeText(this Outcome outcome)
        {
            switch (outcome)
            {
                case Outcome.Pass: return "Passed";
                case Outcome.Fail: return "Failed";
                case Outcome.Inconclusive: return "Inconclusive";
                case Outcome.NotRun:
                default: return "not run";
            }
        }

        private static int ToIndentSizeStepAndBut(this Step step) =>
            step == Step.And || step == Step.But
                ? DefaultIndentSize
                : 0;

        private static int ToIndentSizeStep(this bool hasScenario) =>
            hasScenario
                ? DefaultIndentSize
                : 0;

        private static Some<string> WithIndentation(this in Some<string> prefixedStep, Step step, bool hasScenario)
        {
            var indentSize = hasScenario.ToIndentSizeStep() + step.ToIndentSizeStepAndBut();
            var indent = new string(' ', indentSize);
            return $"{indent}{prefixedStep.Value}";
        }

        private static Some<string> WithOutcomeDescribed(this in Some<string> prefixedStep, Outcome outcome) =>
            $"{prefixedStep.Value} [{outcome.ToOutcomeText()}]";

        private static Some<string> ToPrefix(this Step step) =>
            step.ToString();

        public static Some<string> ToDescription(this StepResult stepResult) =>
            new Option<string>().WithPrefix(stepResult.Step.ToPrefix())
                .WithIndentation(stepResult.Step, hasScenario: false)
                .WithOutcomeDescribed(stepResult.Outcome);

        private static Some<string> ToDescription(this StepOutcome stepOutcome, bool hasScenario) =>
            stepOutcome
                .Text
                .WithPrefix(stepOutcome.Step.ToPrefix())
                .WithIndentation(stepOutcome.Step, hasScenario)
                .WithOutcomeDescribed(stepOutcome.Outcome);

        public static IReadOnlyList<StepResult> ToResults(this IReadOnlyList<StepOutcome> outcomes, bool hasScenario) =>
            outcomes.Select(o => 
                    new StepResult(
                        o.Step,
                        o.Outcome,
                        o.Text.IfNone(null),
                        o.ToDescription(hasScenario)))
                .ToList();

        public static IReadOnlyList<StepOutcome> WithLatestStepOutcomeAs(this IReadOnlyList<StepOutcome> outcomes, Outcome outcome) =>
            outcomes.Select((stepOutcome, index) =>
            {
                var isLastItem = index == outcomes.Count - 1;
                return isLastItem
                    ? stepOutcome.ToStepOutcomeOfOutcome(outcome)
                    : stepOutcome;
            }).ToList();

        private static StepOutcome ToStepOutcomeOfOutcome(this StepOutcome stepOutcome, Outcome outcome) =>
            new StepOutcome(stepOutcome.Step, outcome, stepOutcome.Text);

        public static StepOutcome ToStepOutcome(this in Some<Title> title, Outcome outcome) =>
            new StepOutcome(title.Value.Step, outcome, title.Value.Text);

        public static Some<Title> ToTitle(this string title, Step step) =>
            new Title(step, title);

        private static bool ExceptionTypeNameIsInconclusive(this in Some<string> exceptionTypeName) =>
            string.Equals(exceptionTypeName, "InconclusiveException", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(exceptionTypeName, "AssertInconclusiveException", StringComparison.InvariantCultureIgnoreCase) ||
            (exceptionTypeName.Value.StartsWith("Skippable") && exceptionTypeName.Value.EndsWith("Exception"));

        private static bool ExceptionIsInconclusive(this in Some<Exception> ex) =>
            ExceptionTypeNameIsInconclusive(ex.Value.GetType().Name);

        public static Outcome ToOutcome(this in Some<Exception> ex) =>
            ex.ExceptionIsInconclusive()
                ? Outcome.Inconclusive
                : Outcome.Fail;
    }
}
