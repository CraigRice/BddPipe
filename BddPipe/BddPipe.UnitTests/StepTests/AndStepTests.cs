using BddPipe.UnitTests.Model;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.StepTests
{
    [TestFixture]
    public class AndStepTests
    {
        [Test]
        public void And_WithStepFuncR_UsesR()
        {
            var scenarioResult = Scenario()
                .Given("Initially Model1 is returned", () => new Model1())
                .And("Model2 is now returned as Func<R>", () => new Model2())
                .Then("current instance is now Model2", result => result.GetType().Should().Be(typeof(Model2)))
                .Run();

            var step = scenarioResult.Result.StepResults[1];
            step.Description.Should().Be("    And Model2 is now returned as Func<R> [Passed]");
            step.Outcome.Should().Be(Outcome.Pass);
            step.Step.Should().Be(Step.And);
            step.Title.Should().Be("Model2 is now returned as Func<R>");
        }

        [Test]
        public void And_WithStepFuncTaskR_UsesR()
        {
            var scenarioResult = Scenario()
                .Given("Initially Model1 is returned", () => new Model1())
                .And("Model2 is now returned as Func<R>", async () => new Model2())
                .Then("current instance is now Model2", result => result.GetType().Should().Be(typeof(Model2)))
                .Run();

            var step = scenarioResult.Result.StepResults[1];
            step.Description.Should().Be("    And Model2 is now returned as Func<R> [Passed]");
            step.Outcome.Should().Be(Outcome.Pass);
            step.Step.Should().Be(Step.And);
            step.Title.Should().Be("Model2 is now returned as Func<R>");
        }
    }
}
