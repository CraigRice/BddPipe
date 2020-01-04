using System;
using System.Collections.Generic;
using BddPipe.Model;
using FluentAssertions;

namespace BddPipe.UnitTests.Asserts
{
    internal static class StepOutcomesAsserts
    {
        public static void ShouldHaveOutcomeAtIndex(
            this IReadOnlyList<StepOutcome> stepOutcomes,
            Outcome outcome,
            string text,
            Step step,
            int atIndex
        )
        {
            stepOutcomes.Should().NotBeNull();
            stepOutcomes[atIndex].Outcome.Should().Be(outcome);
            stepOutcomes[atIndex].Text.ShouldBeSome(textValue => textValue.Should().Be(text));
            stepOutcomes[atIndex].Step.Should().Be(step);
        }

        public static void ShouldHaveSingleOutcome(
            this IReadOnlyList<StepOutcome> stepOutcomes,
            Outcome outcome,
            string text,
            Step step
        )
        {
            stepOutcomes.Should().NotBeNull();
            stepOutcomes.Count.Should().Be(1);
            stepOutcomes.ShouldHaveOutcomeAtIndex(outcome, text, step, 0);
        }

        public static void ShouldBeSuccessfulStepWithValue<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, T expectedValue)
        {
            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(expectedValue);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }

        public static void ShouldBeErrorStepWithException<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, Exception expectedException)
        {
            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(expectedException);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Fail, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }

        public static void ShouldBeInconclusiveStepWithException<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, Exception expectedException)
        {
            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(expectedException);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveOutcomeAtIndex(Outcome.Inconclusive, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }
    }
}
