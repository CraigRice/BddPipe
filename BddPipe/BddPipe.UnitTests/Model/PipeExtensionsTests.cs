using System;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using BddPipe.UnitTests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class PipeExtensionsTests
    {
        private const int InitialValue = 1;
        private const int NewValue = 2;
        private const string ErrorMessage = "test message";

        private Pipe<int> GetPipeInErrorState()
        {
            var ex = new ApplicationException("test message");
            return new Ctn<Exception>(ex, None);
        }

        private Pipe<T> GetPipeInSuccessState<T>(T withThisValue)
        {
            return new Ctn<T>(withThisValue, None);
        }

        private void CtnShouldHaveValueState<T>(Ctn<T> ctnValue, T expectedValue)
        {
            ctnValue.Should().NotBeNull();
            ctnValue.Content.Should().Be(expectedValue);
            ctnValue.Should().NotBeNull();
            ctnValue.ScenarioTitle.ShouldBeNone();
            ctnValue.StepOutcomes.Should().BeEmpty();
        }

        private void CtnExceptionShouldHaveErrorState(Ctn<Exception> ctnEx)
        {
            ctnEx.Should().NotBeNull();
            ctnEx.Content.Should().NotBeNull();
            ctnEx.Content.GetType().Should().Be<ApplicationException>();
            ctnEx.Content.Message.Should().Be(ErrorMessage);
            ctnEx.Should().NotBeNull();
            ctnEx.ScenarioTitle.ShouldBeNone();
            ctnEx.StepOutcomes.Should().BeEmpty();
        }

        [Test]
        public void Map_NullArgument_ThrowsArgumentNullException()
        {
            var pipe = GetPipeInSuccessState(NewValue);
            Action map = () => { pipe.Map((Func<int, string>) null); };
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

            Pipe<string> pipe = new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle);

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

            Pipe<string> pipe = new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle);

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
                ctnError.Content.Should().BeOfType<DivideByZeroException>();
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

            Pipe<string> pipe = new Ctn<string>(initalValue, new[]
            {
                new StepOutcome(Step.Given, Outcome.Pass, givenStepText)
            }, scenarioTitle);

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
                ctnError.Content.Should().BeOfType<InconclusiveException>();
                ctnError.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, givenStepText, Step.Given);
            });
        }

        [Test]
        public void Bind_InErrorState_RemainsInErrorStateFunctionNotInvoked()
        {
            var pipe = GetPipeInErrorState();
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

            var result = pipe.Bind(fnBind);

            fnBind.DidNotReceive()(Arg.Any<Ctn<int>>());
            result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void Bind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

            var result = pipe.Bind(fnBind);

            fnBind.Received()(Arg.Any<Ctn<int>>());
            result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        }

        [Test]
        public void Bind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var result = pipe.Bind(fnBind);

            fnBind.Received()(Arg.Any<Ctn<int>>());
            result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void BiBind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInSuccessState(InitialValue));

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
            fnForErrorState.DidNotReceive()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void BiBind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInErrorState());

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
            fnForErrorState.DidNotReceive()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        }

        [Test]
        public void BiBind_InErrorStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInErrorState();

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInErrorState());

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
            fnForErrorState.Received()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        }

        [Test]
        public void BiBind_InErrorStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInErrorState();

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInSuccessState(NewValue));

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
            fnForErrorState.Received()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        }
    }
}
