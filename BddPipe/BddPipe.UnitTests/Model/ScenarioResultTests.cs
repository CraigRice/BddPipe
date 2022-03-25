using System;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class ScenarioResultTests
    {
        [Test]
        public void Ctor_StepResultsNull_ThrowsArgNullException()
        {
            Action call = () => new ScenarioResult(string.Empty, string.Empty, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("stepResults");
        }

        [Test]
        public void Ctor_AllowedNullsNull_CreatesInstance()
        {
            var stepResults = Array.Empty<StepResult>();

            var scenarioResult = new ScenarioResult(null, null, stepResults);
            scenarioResult.Title.Should().Be(null);
            scenarioResult.Description.Should().Be(null);
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEquivalentTo(stepResults);
        }

        [Test]
        public void Ctor_AllSupplied_CreatesInstance()
        {
            const string title = "the title";
            const string description = "the desc";
            var stepResults = Array.Empty<StepResult>();

            var scenarioResult = new ScenarioResult(title, description, stepResults);
            scenarioResult.Title.Should().Be(title);
            scenarioResult.Description.Should().Be(description);
            scenarioResult.StepResults.Should().NotBeNull();
            scenarioResult.StepResults.Should().BeEquivalentTo(stepResults);
        }
    }
}
