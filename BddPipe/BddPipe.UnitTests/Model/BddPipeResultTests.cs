using System.Collections.Generic;
using BddPipe.Model;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model
{
    public class SomeType {}

    [TestFixture]
    public class BddPipeResultTests
    {
        [Test]
        public void Ctor_OutputAndResult_SetsValues()
        {
            var someType = new SomeType();
            var scenarioResult = new ScenarioResult("title", "desc", new List<StepResult>());

            var result = new BddPipeResult<SomeType>(
                someType,
                scenarioResult);

            result.Should().NotBeNull();
            result.Output.Should().Be(someType);
            result.Result.Should().Be(scenarioResult);
        }

        [Test]
        public void Ctor_TNull_AllowsNull()
        {
            SomeType someType = null;
            var scenarioResult = new ScenarioResult("title", "desc", new List<StepResult>());

            var result = new BddPipeResult<SomeType>(
                someType,
                scenarioResult);

            result.Should().NotBeNull();
            result.Output.Should().Be(someType);
            result.Result.Should().Be(scenarioResult);
        }
    }
}
