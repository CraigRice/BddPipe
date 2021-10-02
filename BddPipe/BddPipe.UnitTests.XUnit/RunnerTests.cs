using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.XUnit
{
    public class RunnerTests
    {
        private static IReadOnlyList<string> WriteLogsToConsole(ScenarioResult result)
        {
            Runner.WriteLogsToConsole(result);

            return
                (result.Title == null ? Array.Empty<string>() : new[] { result.Description })
                .Concat(result.StepResults
                    .Select(l => l.Description))
                .ToList();
        }

        [Fact]
        public void Run_FullExampleWithScenario_SuccessfulWithCorrectIndentation()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Scenario("Test scenario")
                .Given(null, () => new { A = 5, B = 10 })
                .And("Nothing", () => { })
                .And("Nothing", () => { })
                .When("the numbers are summed", args => new { Result = args.A + args.B })
                .But("Nothing", () => { })
                .Then("sum should be as expected", arg =>
                {
                    arg.Result.Should().Be(15);
                })
                .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(7);
            logLines[0].Should().Be("Scenario: Test scenario");
            logLines[1].Should().Be("  Given [Passed]");
            logLines[2].Should().Be("    And Nothing [Passed]");
            logLines[3].Should().Be("    And Nothing [Passed]");
            logLines[4].Should().Be("  When the numbers are summed [Passed]");
            logLines[5].Should().Be("    But Nothing [Passed]");
            logLines[6].Should().Be("  Then sum should be as expected [Passed]");
        }

        [Fact]
        public void Run_Example_Successful()
        {
            Scenario()
                .Given("two numbers", () => new { A = 5, B = 10 })
                .When("the numbers are summed", args => new { Result = args.A + args.B })
                .Then("sum should be as expected", arg =>
                {
                    arg.Result.Should().Be(15);
                })
                .Run();
        }
    }
}
