using BddPipe.UnitTests.Asserts;
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
            scenario.ShouldBeSuccessful(ctn =>
            {
                ctn.ScenarioTitle.ShouldBeSome(title =>
                    title.Should().Be(nameof(Scenario_NoTitle_UsesMethodName))
                );
                ctn.StepOutcomes.Should().BeEmpty();
                ctn.Content.Should().NotBeNull();
                ctn.Content.Title.Should().Be(nameof(Scenario_NoTitle_UsesMethodName));
            });
        }

        [Test]
        public void Scenario_ExplicitlyNullTitle_TitleIsNull()
        {
            var scenario = Scenario(null, null);

            scenario.Should().NotBeNull();
            scenario.ShouldBeSuccessful(ctn =>
            {
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.Should().BeEmpty();
                ctn.Content.Should().NotBeNull();
                ctn.Content.Title.Should().Be(null);
            });
        }

        [Test]
        public void Scenario_Title_UsesExplicitTitle()
        {
            const string scenarioTitle = "A Scenario Title";
            var scenario = Scenario(scenarioTitle);

            scenario.Should().NotBeNull();
            scenario.ShouldBeSuccessful(ctn =>
            {
                ctn.ScenarioTitle.ShouldBeSome(title =>
                    title.Should().Be(scenarioTitle)
                );
                ctn.StepOutcomes.Should().BeEmpty();
                ctn.Content.Should().NotBeNull();
                ctn.Content.Title.Should().Be(scenarioTitle);
            });
        }
    }
}
