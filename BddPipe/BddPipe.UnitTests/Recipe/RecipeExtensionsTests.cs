using System;
using BddPipe.Model;
using BddPipe.Recipe;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.Recipe
{
    public sealed class ScenarioInfo
    {
        public string TestValue { get; }

        public ScenarioInfo(string testValue)
        {
            TestValue = testValue;
        }
    }

    public sealed class ScenarioInfo2
    {
        public string TestValue { get; }

        public ScenarioInfo2(string testValue)
        {
            TestValue = testValue;
        }
    }

    public static class Recipe
    {
        public const string AnyStringArg = "any-arg";
        public const string GivenStepTitle = "given-step-text";

        public static Func<Scenario, Pipe<ScenarioInfo>> SetupWithScenario() =>
            scenario =>
                scenario.Given(GivenStepTitle, () => new ScenarioInfo(AnyStringArg));

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo>> SetupWithScenarioInfo() =>
            pipe =>
                pipe.And("", scenarioInfo => new ScenarioInfo(scenarioInfo + " extra"));

        public static Func<Pipe<ScenarioInfo>, Pipe<ScenarioInfo2>> SetupWithDifferentScenarioInfo() =>
            pipe =>
                pipe.And("", scenarioInfo => new ScenarioInfo2(scenarioInfo + " extra"));

    }

    [TestFixture]
    public class RecipeExtensionsTests
    {
        private const string ScenarioText = "scenario-text";

        [Test]
        public void GivenRecipe_WithScenario_AppliesGivenStep()
        {
            var scenarioSetup = Scenario(ScenarioText)
                .GivenRecipe(Recipe.SetupWithScenario());

            scenarioSetup.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().NotBeNull();
                ctn.Content.TestValue.Should().Be(Recipe.AnyStringArg);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleOutcome(Outcome.Pass, Recipe.GivenStepTitle, Step.Given);
            });
        }
    }
}
