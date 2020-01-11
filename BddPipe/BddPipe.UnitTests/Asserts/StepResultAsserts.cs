using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;

namespace BddPipe.UnitTests.Asserts
{
    public static class StepResultAsserts
    {
        public static void ShouldHaveOutcomeAtIndex(
            this IReadOnlyList<StepResult> stepOutcomes,
            Outcome outcome,
            string title,
            string description,
            Step step,
            int atIndex
        )
        {
            stepOutcomes.Should().NotBeNull();
            stepOutcomes[atIndex].Outcome.Should().Be(outcome);
            stepOutcomes[atIndex].Title.Should().Be(title);
            stepOutcomes[atIndex].Description.Should().Be(description);
            stepOutcomes[atIndex].Step.Should().Be(step);
        }
    }
}
