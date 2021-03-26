using System;
using System.Collections.Generic;
using BddPipe.Model;
using FluentAssertions;

namespace BddPipe.UnitTests.Asserts
{
    internal static class StepOutcomesAsserts
    {
        public static void ShouldHaveStepOutcomeAtIndex(
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

        public static void ShouldHaveSingleStepOutcome(
            this IReadOnlyList<StepOutcome> stepOutcomes,
            Outcome outcome,
            string text,
            Step step
        )
        {
            stepOutcomes.Should().NotBeNull();
            stepOutcomes.Count.Should().Be(1);
            stepOutcomes.ShouldHaveStepOutcomeAtIndex(outcome, text, step, 0);
        }

        public static void ShouldBeSuccessfulGivenStepWithValue<T>(this Pipe<T> step, string givenTitle, T expectedValue)
        {
            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(expectedValue);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.Count.Should().Be(1);
            });
        }

        public static void ShouldBeSuccessfulSecondStepWithValue<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, T expectedValue)
        {
            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(expectedValue);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }

        public static void ShouldBeErrorSecondStepWithException<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, Exception expectedException)
        {
            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(expectedException);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Fail, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }

        public static void ShouldBeInconclusiveSecondStepWithException<T>(this Pipe<T> step, Step stepType, string givenTitle, string expectedTitle, Exception expectedException)
        {
            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(expectedException);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Pass, givenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Inconclusive, expectedTitle, stepType, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }
    }
}
