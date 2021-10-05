using System;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void CreatePipe_NoArgs_ReturnsPipeInEmptyState()
        {
            var pipe = Runner.CreatePipe();
            pipe.ShouldBeSuccessful(ctnScenario =>
            {
                ctnScenario.Content.Should().NotBeNull();
                ctnScenario.Content.Should().BeOfType<Unit>();
                ctnScenario.ScenarioTitle.ShouldBeNone();
                ctnScenario.StepOutcomes.Should().NotBeNull();
                ctnScenario.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void CreatePipe_ScenarioNull_ThrowsArgNullException()
        {
            Action call = () => Runner.CreatePipe(null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void CreatePipe_WithScenarioHavingNullTitle_ReturnsExpectedPipe()
        {
            var pipe = Runner.CreatePipe(new Scenario(title: null));
            pipe.ShouldBeSuccessful(ctnScenario =>
            {
                ctnScenario.Content.Should().NotBeNull();
                ctnScenario.Content.Title.Should().BeNull();
                ctnScenario.ScenarioTitle.ShouldBeNone();
                ctnScenario.StepOutcomes.Should().NotBeNull();
                ctnScenario.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void CreatePipe_WithScenarioHavingTitle_ReturnsExpectedPipe()
        {
            const string scenarioTitle = "a title";

            var pipe = Runner.CreatePipe(new Scenario(title: scenarioTitle));
            pipe.ShouldBeSuccessful(ctnScenario =>
            {
                ctnScenario.Content.Should().NotBeNull();
                ctnScenario.Content.Title.Should().Be(scenarioTitle);
                ctnScenario.ScenarioTitle.ShouldBeSome(title =>
                {
                    title.Should().Be(scenarioTitle);
                });
                ctnScenario.StepOutcomes.Should().NotBeNull();
                ctnScenario.StepOutcomes.Should().BeEmpty();
            });
        }
    }
}
