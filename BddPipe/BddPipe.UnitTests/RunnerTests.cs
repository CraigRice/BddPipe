using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using BddPipe.UnitTests.Helpers;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
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

        [Test]
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

        [Test]
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

        [Test]
        public void Run_GivenWhenThenAndAsActions_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            int a = 0, b = 0;
            int result = 0;

            Given("Two numbers", () =>
            {
                a = 6;
                b = 20;
            })
            .When("Numbers are summed", () => result = a + b)
            .Then("Result is correct", () => result.Should().Be(26))
            .And("Result is not zero", () => result.Should().NotBe(0))
            .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When Numbers are summed [Passed]");
            logLines[2].Should().Be("Then Result is correct [Passed]");
            logLines[3].Should().Be("  And Result is not zero [Passed]");
        }

        [Test]
        public void Run_GivenOnly_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Given("Two numbers", () => new { A = 5, B = 10 })
                .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(1);
            logLines[0].Should().Be("Given Two numbers [Passed]");
        }

        [Test]
        public void Run_GivenWhenThen_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Given("Two numbers", () => new { A = 5, B = 10 }).
                When("The numbers are summed", args => new { Result = args.A + args.B }).
                Then("Sum should be as expected", arg =>
                {
                    arg.Result.Should().Be(15);
                })
                .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(3);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Passed]");
            logLines[2].Should().Be("Then Sum should be as expected [Passed]");
        }

        [Test]
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

            runTest.Should().Throw<InconclusiveException>().Where(ex => ex.Message == "Inconclusive message");

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Passed]");
            logLines[2].Should().Be("  And Inconclusive is raised [Inconclusive]");
            logLines[3].Should().Be("Then Sum should be as expected [not run]");
        }

        [Test]
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

            runTest.Should().Throw<InconclusiveException>().Where(ex => ex.Message == "Inconclusive message");

            logLines.Count.Should().Be(3);
            logLines[0].Should().Be("Given Two numbers [Inconclusive]");
            logLines[1].Should().Be("When The numbers are summed [not run]");
            logLines[2].Should().Be("Then Sum should be as expected [not run]");
        }

        [Test]
        public void Run_GivenWhenAsyncThen_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Given("Two numbers", () => new { A = 5, B = 10 }).
                When("The numbers are summed", async args =>
                {
                    await Task.Delay(10);
                    return new { Result = args.A + args.B };
                }).
                Then("Sum should be as expected", arg =>
                {
                    arg.Result.Should().Be(15);
                })
                .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(3);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Passed]");
            logLines[2].Should().Be("Then Sum should be as expected [Passed]");
        }

        [Test]
        public void Run_GivenWhenThenAnd_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Given("Two numbers", () => new { A = 5, B = 10 }).
                When("The numbers are summed", args => new { Result = args.A + args.B }).
                Then("Sum is not zero", arg =>
                {
                    arg.Result.Should().NotBe(0);
                })
                .And("Sum should be as expected", arg =>
                {
                    arg.Result.Should().Be(15);
                })
                .Run(logs => logLines = WriteLogsToConsole(logs));

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Passed]");
            logLines[2].Should().Be("Then Sum is not zero [Passed]");
            logLines[3].Should().Be("  And Sum should be as expected [Passed]");
        }

        [Test]
        public void Run_GivenWhenThenAndHavingWhenFail_LogIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Given("Two numbers", () => new { A = 5, B = 10 }).
                    When("The numbers are summed", args =>
                    {
                        throw new ApplicationException("test error");
                        return new { Result = args.A + args.B };
                    }).
                    Then("Sum is not zero", arg =>
                    {
                        arg.Result.Should().NotBe(0);
                    })
                    .And("Sum should be as expected", arg =>
                    {
                        arg.Result.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<ApplicationException>();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Given Two numbers [Passed]");
            logLines[1].Should().Be("When The numbers are summed [Failed]");
            logLines[2].Should().Be("Then Sum is not zero [not run]");
            logLines[3].Should().Be("  And Sum should be as expected [not run]");
        }

        [Test]
        public void Run_AssignExternalVarsViaTask_ResultSet()
        {
            int a = 0, b = 0;
            int result = 0;

            Given("two numbers", () =>
            {
                a = 6;
                b = 20;
            })
            .When("the numbers are summed", async () =>
            {
                await Task.Delay(10);
                result = a + b;
            })
            .Then("sum should be as expected", () =>
            {
                result.Should().Be(26);
            })
            .Run();
        }

        [Test]
        public void Run_FuncTaskAsyncCallThrowsException_RaisesThrownExNotAggregateException()
        {
            Action runTest = () =>
            {
                Given("Async step throws exception", async () =>
                {
                    await Task.Delay(1);
                    throw new ApplicationException("test exception");
                })
                .Run();
            };

            runTest.Should().ThrowExactly<ApplicationException>().WithMessage("test exception");
        }

        [Test]
        public void Run_FuncTaskOfTAsyncCallThrowsException_RaisesThrownExNotAggregateException()
        {
            Action runTest = () =>
            {
                Given("Async step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                        return 5;
                    })
                    .Run();
            };

            runTest.Should().ThrowExactly<ApplicationException>().WithMessage("test exception");
        }

        [Test]
        public void Run_ScenarioTitleExplicitlyNull_ScenarioIsIgnored()
        {
            const string title = "Scenario title is null";
            var bddPipeResult = Scenario(null, null)
                .Given(title, () => { })
                .Run();

            bddPipeResult.Result.Should().NotBeNull();
            bddPipeResult.Result.Title.Should().BeNull();
            bddPipeResult.Result.Description.Should().Be("Scenario:");
            bddPipeResult.Result.StepResults.Should().NotBeNull();
            bddPipeResult.Result.StepResults.Count.Should().Be(1);
            bddPipeResult.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, title, $"Given {title} [Passed]", Step.Given, 0);
        }

        [Test]
        public void Map_AfterGivenWhen_PipeIsInCorrectState()
        {
            const string givenTitle = "the given title";
            const string whenTitle = "the when title";
            const string valueInComplexType = "the string value";

            var pipe = Scenario()
                .Given(givenTitle, () => { })
                .When(whenTitle, () => new { A = 1, B = valueInComplexType })
                .Map(stepValue => stepValue.B);

            pipe.ShouldBeSuccessfulSecondStepWithValue(Step.When, givenTitle, whenTitle, valueInComplexType);
        }

        [Test]
        public void Map_AsyncMap_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(async text =>
                    {
                        await Task.Delay(10);
                        return text.Length;
                    })
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(4);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().NotThrow();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_AsyncMap_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Passed]");
            logLines[3].Should().Be("  Then text length should be as expected [Passed]");
        }

        [Test]
        public void Map_SyncMap_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(text => text.Length)
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(4);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().NotThrow();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_SyncMap_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Passed]");
            logLines[3].Should().Be("  Then text length should be as expected [Passed]");
        }

        [Test]
        public void Map_DoubleMap_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(text => text.Length)
                    .Map(length => length.ToString())
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be("4");
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().NotThrow();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_DoubleMap_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Passed]");
            logLines[3].Should().Be("  Then text length should be as expected [Passed]");
        }

        [Test]
        public void Map_AsyncMapThrowsInconclusiveException_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(PipeMapFunctions.MapAsyncRaiseInconclusiveEx())
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<InconclusiveException>();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_AsyncMapThrowsInconclusiveException_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Inconclusive]");
            logLines[3].Should().Be("  Then text length should be as expected [not run]");
        }

        [Test]
        public void Map_SyncMapThrowsInconclusiveException_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(PipeMapFunctions.MapSyncRaiseInconclusiveEx())
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<InconclusiveException>();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_SyncMapThrowsInconclusiveException_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Inconclusive]");
            logLines[3].Should().Be("  Then text length should be as expected [not run]");
        }

        [Test]
        public void Map_AsyncMapThrowsException_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(PipeMapFunctions.MapAsyncRaiseEx())
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<DivideByZeroException>();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_AsyncMapThrowsException_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Failed]");
            logLines[3].Should().Be("  Then text length should be as expected [not run]");
        }

        [Test]
        public void Map_SyncMapThrowsException_OutputIsCorrect()
        {
            IReadOnlyList<string> logLines = new List<string>();

            Action runTest = () =>
                Scenario()
                    .Given("some text", () => "some text")
                    .When("the text is altered", text => text.Substring(0, 4))
                    .Map(PipeMapFunctions.MapSyncRaiseEx())
                    .Then("text length should be as expected", arg =>
                    {
                        arg.Should().Be(15);
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));

            runTest.Should().Throw<DivideByZeroException>();

            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: Map_SyncMapThrowsException_OutputIsCorrect");
            logLines[1].Should().Be("  Given some text [Passed]");
            logLines[2].Should().Be("  When the text is altered [Failed]");
            logLines[3].Should().Be("  Then text length should be as expected [not run]");
        }
    }
}
