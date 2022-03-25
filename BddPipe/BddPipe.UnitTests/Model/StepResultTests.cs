using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class StepResultTests
    {
        [Test]
        public void Ctor_AllowedNullsNull_CreatesInstance()
        {
            var stepResult = new StepResult(Step.And, Outcome.Fail, null, null);

            stepResult.Step.Should().Be(Step.And);
            stepResult.Outcome.Should().Be(Outcome.Fail);
            stepResult.Title.Should().Be(null);
            stepResult.Description.Should().Be(null);
        }

        [Test]
        public void Ctor_AllSupplied_CreatesInstance()
        {
            const string title = "the title";
            const string description = "the desc";

            var stepResult = new StepResult(Step.And, Outcome.Fail, title, description);

            stepResult.Step.Should().Be(Step.And);
            stepResult.Outcome.Should().Be(Outcome.Fail);
            stepResult.Title.Should().Be(title);
            stepResult.Description.Should().Be(description);
        }
    }
}
