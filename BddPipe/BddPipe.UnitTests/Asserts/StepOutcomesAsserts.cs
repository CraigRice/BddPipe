using System.Collections.Generic;
using FluentAssertions;

namespace BddPipe.UnitTests.Asserts
{
    internal static class StepOutcomesAsserts
    {
        public static void ShouldHaveSingleOutcome(
            this IReadOnlyList<StepOutcome> stepOutcomes,
            Outcome outcome,
            string text,
            Step step
        )
        {
            stepOutcomes.Should().NotBeNull();
            stepOutcomes.Count.Should().Be(1);
            stepOutcomes[0].Outcome.Should().Be(outcome);
            stepOutcomes[0].Text.ShouldBeSome(textValue => textValue.Should().Be(text));
            stepOutcomes[0].Step.Should().Be(step);
        }
    }
}
