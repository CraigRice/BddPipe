using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using BddPipe.UnitTests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerMapTests
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

        private const int InitialValue = 1;
        private const int NewValue = 2;
        private const string ErrorMessage = "test message";

        private Pipe<int> GetPipeInErrorState()
        {
            var ex = new ApplicationException("test message");
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            return new Pipe<int>(new Ctn<ExceptionDispatchInfo>(exInfo, None));
        }

        private Pipe<T> GetPipeInSuccessState<T>(T withThisValue)
        {
            return new Pipe<T>(new Ctn<T>(withThisValue, None));
        }

        private void CtnShouldHaveValueState<T>(Ctn<T> ctnValue, T expectedValue)
        {
            ctnValue.Should().NotBeNull();
            ctnValue.Content.Should().Be(expectedValue);
            ctnValue.Should().NotBeNull();
            ctnValue.ScenarioTitle.ShouldBeNone();
            ctnValue.StepOutcomes.Should().BeEmpty();
        }

        private void CtnExceptionShouldHaveErrorState(Ctn<ExceptionDispatchInfo> ctnEx)
        {
            ctnEx.Should().NotBeNull();
            ctnEx.Content.Should().NotBeNull();
            ctnEx.Content.SourceException.Should().NotBeNull();
            ctnEx.Content.SourceException.GetType().Should().Be<ApplicationException>();
            ctnEx.Content.SourceException.Message.Should().Be(ErrorMessage);
            ctnEx.Should().NotBeNull();
            ctnEx.ScenarioTitle.ShouldBeNone();
            ctnEx.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public void Map_NullArgument_ThrowsArgumentNullException()
        {
            var pipe = GetPipeInSuccessState(NewValue);
            Action map = () => { pipe.Map((Func<int, string>)null); };
            map.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void MapAsync_NullArgument_ThrowsArgumentNullException()
        {
            var pipe = GetPipeInSuccessState(NewValue);
            Action map = () => { pipe.Map((Func<int, Task<string>>)null); };
            map.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void Map_IsInErrorStateToChangedType_MapFunctionIsNotCalled()
        {
            const string nextValue = "next value";
            var pipe = GetPipeInErrorState();

            var fnMap = Substitute.For<Func<int, string>>();
            fnMap(Arg.Any<int>()).Returns(nextValue);

            var asString = pipe.Map(fnMap);

            fnMap.DidNotReceive()(Arg.Any<int>());
            asString.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void Map_IsInValueStateToChangedType_MapFunctionIsCalled()
        {
            const string nextValue = "next value";
            var pipe = GetPipeInSuccessState(NewValue);

            var fnMap = Substitute.For<Func<int, string>>();
            fnMap(Arg.Any<int>()).Returns(nextValue);

            var asString = pipe.Map(fnMap);

            fnMap.Received()(Arg.Any<int>());
            asString.ShouldBeSuccessful(ctn => CtnShouldHaveValueState(ctn, nextValue));
        }

        [Test]
        public void Map_IsInErrorStateToChangedType_IsStillInErrorState()
        {
            var pipe = GetPipeInErrorState();

            var asString = pipe.Map(i => i.ToString());

            asString.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void Map_IsInValueStateToChangedType_MapsToCtnOfNewValue()
        {
            var pipe = GetPipeInSuccessState(NewValue);

            var asString = pipe.Map(i => i.ToString());

            asString.ShouldBeSuccessful(ctn =>
            {
                CtnShouldHaveValueState(ctn, NewValue.ToString());
            });
        }

        [Test]
        public void Map_IsInValueStateToSameType_MapsToCtnOfNewValue()
        {
            var pipe = GetPipeInSuccessState(NewValue);

            var asString = pipe.Map(i => i + 10);

            asString.ShouldBeSuccessful(ctn =>
            {
                CtnShouldHaveValueState(ctn, NewValue + 10);
            });
        }

        [Test]
        public void Map_IsInValueStateToNull_MapsToCtnOfNewValue()
        {
            var pipe = GetPipeInSuccessState(NewValue);

            var asString = pipe.Map(i => (string)null);

            asString.ShouldBeSuccessful(ctn =>
            {
                CtnShouldHaveValueState(ctn, null);
            });
        }

        [Test]
        public void Map_IsInValueStateFromNull_MapsToCtnOfNewValue()
        {
            var pipe = GetPipeInSuccessState((string)null);
            const int nextValue = 423;

            var asString = pipe.Map(i => nextValue);

            asString.ShouldBeSuccessful(ctn =>
            {
                CtnShouldHaveValueState(ctn, nextValue);
            });
        }

        [Test]
        public void Map_AsyncOverload_MapsToCtnOfNewValue()
        {
            const string initalValue = "initial-value";
            const string scenarioTitle = "scenario-title";
            const string givenStepText = "given-step";

            var pipe = new Pipe<string>(new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle));

            var result = pipe.Map(async value =>
            {
                await Task.Delay(10);
                return new DateTime(2000, 1, 1, 1, 1, 1);
            });

            result.ShouldBeSuccessful(ctnValue =>
            {
                ctnValue.ScenarioTitle.ShouldBeSome(title => title.Should().Be(scenarioTitle));
                ctnValue.Content.Should().Be(new DateTime(2000, 1, 1, 1, 1, 1));
                ctnValue.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, givenStepText, Step.Given);
            });
        }

        [TestCase(true, Description = "Async overload")]
        [TestCase(false, Description = "Sync overload")]
        public void Map_ThrowsException_PipeIsInErrorStateWithFail(bool isAsync)
        {
            const string initalValue = "initial-value";
            const string scenarioTitle = "scenario-title";
            const string givenStepText = "given-step";

            var pipe = new Pipe<string>(new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle));

            Pipe<int> result;
            if (isAsync)
            {
                result = pipe.Map(PipeMapFunctions.MapAsyncRaiseEx());
            }
            else
            {
                result = pipe.Map(PipeMapFunctions.MapSyncRaiseEx());
            }

            result.ShouldBeError(ctnError =>
            {
                ctnError.ScenarioTitle.ShouldBeSome(title => title.Should().Be(scenarioTitle));
                ctnError.Content.Should().NotBeNull();
                ctnError.Content.SourceException.Should().NotBeNull();
                ctnError.Content.SourceException.Should().BeOfType<DivideByZeroException>();
                ctnError.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, givenStepText, Step.Given);
            });
        }

        [TestCase(true, Description = "Async overload")]
        [TestCase(false, Description = "Sync overload")]
        public void Map_ThrowsInconclusiveException_PipeIsInErrorStateWithInconclusive(bool isAsync)
        {
            const string initalValue = "initial-value";
            const string scenarioTitle = "scenario-title";
            const string givenStepText = "given-step";

            var pipe = new Pipe<string>(new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle));

            Pipe<int> result;

            if (isAsync)
            {
                result = pipe.Map(PipeMapFunctions.MapAsyncRaiseInconclusiveEx());
            }
            else
            {
                result = pipe.Map(PipeMapFunctions.MapSyncRaiseInconclusiveEx());
            }

            result.ShouldBeError(ctnError =>
            {
                ctnError.ScenarioTitle.ShouldBeSome(title => title.Should().Be(scenarioTitle));
                ctnError.Content.Should().NotBeNull();
                ctnError.Content.SourceException.Should().NotBeNull();
                ctnError.Content.SourceException.Should().BeOfType<InconclusiveException>();
                ctnError.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, givenStepText, Step.Given);
            });
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
