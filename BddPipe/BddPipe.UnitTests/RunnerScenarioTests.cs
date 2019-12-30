using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerScenarioTests
    {
        [Test]
        public void Scenario_NoTitle_UsesMethodName()
        {
            var scenario = Scenario();

            scenario.Should().NotBeNull();
            scenario.Title.Should().Be(nameof(Scenario_NoTitle_UsesMethodName));
        }

        [Test]
        public void Scenario_Title_UsesExplicitTitle()
        {
            const string scenarioTitle = "A Scenario Title";
            var scenario = Scenario(scenarioTitle);

            scenario.Should().NotBeNull();
            scenario.Title.Should().Be(scenarioTitle);
        }
    }
}
