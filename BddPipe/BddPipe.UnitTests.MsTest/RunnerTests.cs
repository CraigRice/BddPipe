using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.MsTest
{
    [TestClass]
    public class RunnerTests
    {
        private static IReadOnlyList<string> WriteLogsToConsole(ScenarioResult result)
        {
            Runner.WriteLogsToConsole(result);

            return
                (result.Title == null ? new string[0] : new[] { result.Description })
                .Concat(result.StepResults
                .Select(l => l.Description))
                .ToList();
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void Run_GivenWhenAndThenWithAndAssertInconclusive_IndicatedInconclusive()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
                Given("Two numbers", () => new { A = 5, B = 10 }).
                    When("The numbers are summed", args => new { Result = args.A + args.B }).
                    And("Inconclusive is raised", () => Assert.Inconclusive("Inconclusive message")).
                    Then("Sum should be as expected", arg =>
                    {
                        arg.Result.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<AssertInconclusiveException>().Where(ex => ex.Message == "Assert.Inconclusive failed. Inconclusive message");

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Passed]");
            logLines[2].Should().Be("  And Inconclusive is raised [Inconclusive]");
            logLines[3].Should().Be("Then Sum should be as expected [not run]");
        }

        [TestMethod]
        public void Run_GivenWhenThenWithGivenAssertInconclusive_IndicatedInconclusive()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Given("Two numbers", () =>
                {
                    Assert.Inconclusive("Inconclusive message");
                    return new { A = 5, B = 10 };
                }).
                    When("The numbers are summed", args => new { Result = args.A + args.B }).
                    Then("Sum should be as expected", arg =>
                    {
                        arg.Result.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<AssertInconclusiveException>().Where(ex => ex.Message == "Assert.Inconclusive failed. Inconclusive message");

            logLines.Count.Should().Be(3);
            logLines[0].Should().Be("Given Two numbers [Inconclusive]");
            logLines[1].Should().Be("When The numbers are summed [not run]");
            logLines[2].Should().Be("Then Sum should be as expected [not run]");
        }
    }
}
