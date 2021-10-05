using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerRunTests
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

        [Test]
        public void WriteLogsToConsole_ResultNull_ThrowsArgNullException()
        {
            Action call = () => Runner.WriteLogsToConsole(null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("result");
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
        public async Task RunAsync_GivenWhenThenAndAsActions_Success()
        {
            IReadOnlyList<string> logLines = new List<string>();

            int a = 0, b = 0;
            int result = 0;

            await Given("Two numbers", () =>
                {
                    a = 6;
                    b = 20;
                })
                .When("Numbers are summed", () => result = a + b)
                .Then("Result is correct", () => result.Should().Be(26))
                .And("Result is not zero", () => result.Should().NotBe(0))
                .RunAsync(logs => logLines = WriteLogsToConsole(logs));

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
        public async Task RunAsync_FuncTaskAsyncCallThrowsException_RaisesThrownExNotAggregateException()
        {
            Func<Task> runTest = async () =>
            {
                await Given("Async step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                    })
                    .RunAsync();
            };

            (await runTest.Should().ThrowExactlyAsync<ApplicationException>())
                .WithMessage("test exception");
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

        private static void ShouldHaveExpectedResultFromAsyncSyncComparisonTest(BddPipeResult<bool> bddPipeResult)
        {
            bddPipeResult.Should().NotBeNull();
            bddPipeResult.Output.Should().BeTrue();
            bddPipeResult.Result.Should().NotBeNull();
            bddPipeResult.Result.Title.Should().NotBeNull();
            bddPipeResult.Result.Description.Should().NotBeNull();
            bddPipeResult.Result.StepResults.Should().NotBeNull();
            bddPipeResult.Result.StepResults.Count.Should().Be(2);
            bddPipeResult.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "false", "  Given false [Passed]", Step.Given, 0);
            bddPipeResult.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "not false", "  When not false [Passed]", Step.When, 1);
        }

        private static void ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(IReadOnlyList<string> logLines)
        {
            logLines.Should().NotBeNull();
            logLines.Count.Should().Be(4);
            logLines[0].Should().Be("Scenario: test");
            logLines[1].Should().Be("  Given false [Passed]");
            logLines[2].Should().Be("  When Step throws exception [Failed]");
            logLines[3].Should().Be("  Then final step [not run]");
        }


        /// <summary>
        /// Given - async
        /// When - sync
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenAsyncWhenSync_HasCorrectResult()
        {
            var bddPipeResult = await Scenario()
                .Given("false", async scenario =>
                {
                    await Task.Delay(1);
                    return false;
                })
                .When("not false", falseValue => !falseValue)
                .RunAsync();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - async
        /// When - sync
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenAsyncWhenSync_HasCorrectResult()
        {
            var bddPipeResult = Scenario()
                .Given("false", async scenario =>
                {
                    await Task.Delay(1);
                    return false;
                })
                .When("not false", falseValue => !falseValue)
                .Run();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - async
        /// When - sync THROWS
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenAsyncWhenSyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Func<Task> runTest = async () =>
            {
                await Scenario("test")
                    .Given("false", async scenario =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .When("Step throws exception", () => throw new ApplicationException("test exception"))
                    .Then("final step", val => !val)
                    .RunAsync(logs => logLines = WriteLogsToConsole(logs));
            };

            (await runTest.Should().ThrowExactlyAsync<ApplicationException>())
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - async
        /// When - sync THROWS
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenAsyncWhenSyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
            {
                Scenario("test")
                    .Given("false", async scenario =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .When("Step throws exception", () => throw new ApplicationException("test exception"))
                    .Then("final step", val => !val)
                    .Run(logs => logLines = WriteLogsToConsole(logs));
            };

            runTest.Should().ThrowExactly<ApplicationException>()
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - async
        /// When - sync THROWS
        /// </summary>
        /// <remarks>Run - the Then step is following an a step in a failed state and is async</remarks>
        [Test]
        public void Run_AsyncStepFollowsWhenInFailedAsyncMode_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
            {
                Scenario("test")
                    .Given("false", async scenario =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .When("Step throws exception", () => throw new ApplicationException("test exception"))
                    .Then("final step", async val =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .Run(logs => logLines = WriteLogsToConsole(logs));
            };

            runTest.Should().ThrowExactly<ApplicationException>()
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - async
        /// When - async
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenAsyncWhenAsync_HasCorrectResult()
        {
            var bddPipeResult = await Scenario()
                .Given("false", async scenario =>
                {
                    await Task.Delay(1);
                    return false;
                })
                .When("not false", async falseValue =>
                {
                    await Task.Delay(1);
                    return !falseValue;
                })
                .RunAsync();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - async
        /// When - async
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenAsyncWhenAsync_HasCorrectResult()
        {
            var bddPipeResult = Scenario()
                .Given("false", async scenario =>
                {
                    await Task.Delay(1);
                    return false;
                })
                .When("not false", async falseValue =>
                {
                    await Task.Delay(1);
                    return !falseValue;
                })
                .Run();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - async
        /// When - async THROWS
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenAsyncWhenAsyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Func<Task> runTest = async () =>
            {
                await Scenario("test")
                    .Given("false", async scenario =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .When("Step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                    })
                    .Then("final step", val => !val)
                    .RunAsync(logs => logLines = WriteLogsToConsole(logs));
            };

            (await runTest.Should().ThrowExactlyAsync<ApplicationException>())
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - async
        /// When - async THROWS
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenAsyncWhenAsyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
            {
                Scenario("test")
                    .Given("false", async scenario =>
                    {
                        await Task.Delay(1);
                        return false;
                    })
                    .When("Step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                    })
                    .Then("final step", val => !val)
                    .Run(logs => logLines = WriteLogsToConsole(logs));
            };

            runTest.Should().ThrowExactly<ApplicationException>()
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - sync
        /// When - sync
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenSyncWhenSync_HasCorrectResult()
        {
            var bddPipeResult = await Scenario()
                .Given("false", scenario => false)
                .When("not false", falseValue => !falseValue)
                .RunAsync();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - sync
        /// When - sync
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenSyncWhenSync_HasCorrectResult()
        {
            var bddPipeResult = Scenario()
                .Given("false", scenario => false)
                .When("not false", falseValue => !falseValue)
                .Run();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - sync
        /// When - sync THROWS
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenSyncWhenSyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Func<Task> runTest = async () =>
            {
                await Scenario("test")
                    .Given("false", scenario => false)
                    .When("Step throws exception", () => throw new ApplicationException("test exception"))
                    .Then("final step", val => !val)
                    .RunAsync(logs => logLines = WriteLogsToConsole(logs));
            };

            (await runTest.Should().ThrowExactlyAsync<ApplicationException>())
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - sync
        /// When - sync THROWS
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenSyncWhenSyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
            {
                Scenario("test")
                    .Given("false", scenario => false)
                    .When("Step throws exception", () => throw new ApplicationException("test exception"))
                    .Then("final step", val => !val)
                    .Run(logs => logLines = WriteLogsToConsole(logs));
            };

            runTest.Should().ThrowExactly<ApplicationException>()
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - sync
        /// When - async
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenSyncWhenAsync_HasCorrectResult()
        {
            var bddPipeResult = await Scenario()
                .Given("false", scenario => false)
                .When("not false", async falseValue =>
                {
                    await Task.Delay(1);
                    return !falseValue;
                })
                .RunAsync();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - sync
        /// When - async
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenSyncWhenAsync_HasCorrectResult()
        {
            var bddPipeResult = Scenario()
                .Given("false", scenario => false)
                .When("not false", async falseValue =>
                {
                    await Task.Delay(1);
                    return !falseValue;
                })
                .Run();

            ShouldHaveExpectedResultFromAsyncSyncComparisonTest(bddPipeResult);
        }

        /// <summary>
        /// Given - sync
        /// When - async THROWS
        /// </summary>
        /// <remarks>Run</remarks>
        [Test]
        public void Run_GivenSyncWhenAsyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Action runTest = () =>
            {
                Scenario("test")
                    .Given("false", async scenario => false)
                    .When("Step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                    })
                    .Then("final step", val => !val)
                    .Run(logs => logLines = WriteLogsToConsole(logs));
            };

            runTest.Should().ThrowExactly<ApplicationException>()
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        /// <summary>
        /// Given - sync
        /// When - async THROWS
        /// </summary>
        /// <remarks>RunAsync</remarks>
        [Test]
        public async Task RunAsync_GivenSyncWhenAsyncThrowsException_RaisesThrownExNotAggregateException()
        {
            IReadOnlyList<string> logLines = new List<string>();
            Func<Task> runTest = async () =>
            {
                await Scenario("test")
                    .Given("false", async scenario => false)
                    .When("Step throws exception", async () =>
                    {
                        await Task.Delay(1);
                        throw new ApplicationException("test exception");
                    })
                    .Then("final step", val => !val)
                    .RunAsync(logs => logLines = WriteLogsToConsole(logs));
            };

            (await runTest.Should().ThrowExactlyAsync<ApplicationException>())
                .WithMessage("test exception");

            ShouldHaveExpectedLogsFromAsyncSyncComparisonTest(logLines);
        }

        [Test]
        public void Run_WhenPipeContentNull_ReturnsResultWithNull()
        {
            var result = Given("content is null", () => (string)null).Run();

            result.Should().NotBeNull();
            result.Output.Should().BeNull();
            result.Result.Should().NotBeNull();
            result.Result.Title.Should().BeNull();
            result.Result.Description.Should().NotBeNull();
            result.Result.StepResults.Should().NotBeNull();
            result.Result.StepResults.Count.Should().Be(1);
            result.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "content is null", "Given content is null [Passed]", Step.Given, 0);
        }

        [Test]
        public async Task RunAsync_WhenPipeContentNull_ReturnsResultWithNull()
        {
            var result = await Given("content is null", () => (string)null).RunAsync();

            result.Should().NotBeNull();
            result.Output.Should().BeNull();
            result.Result.Should().NotBeNull();
            result.Result.Title.Should().BeNull();
            result.Result.Description.Should().NotBeNull();
            result.Result.StepResults.Should().NotBeNull();
            result.Result.StepResults.Count.Should().Be(1);
            result.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "content is null", "Given content is null [Passed]", Step.Given, 0);
        }
    }
}
