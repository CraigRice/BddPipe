using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.UnitTests.Model.PipeTests.PipeTestsHelper;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void Match_DefaultPipe_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                default(Pipe<int>).Match(v => DefaultValue, e => DefaultValue);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void Match_DefaultPipeWithAction_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                default(Pipe<int>).Match(v => {}, e => {});
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionRight_CallsActionCtnT(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnT = Substitute.For<Action<PipeState<int>>>();
            var fnError = Substitute.For<Action<PipeErrorState>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Any<PipeState<int>>());
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionLeft_CallsActionCtnError(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Action<PipeState<int>>>();
            var fnError = Substitute.For<Action<PipeErrorState>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionRightNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnError = Substitute.For<Action<PipeErrorState>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionLeftNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Action<PipeState<int>>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRight_CallsFuncRight(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.Received()(Arg.Any<PipeState<int>>());
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeft_CallsFuncLeft(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();
            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            pipe.Match(fnT, fnError);

            fnT.DidNotReceive();
            fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRight_ReturnsFuncRightOutput(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnError = Substitute.For<Func<PipeErrorState, string>>();

            const string resultText = "some result";
            var result = pipe.Match(ctnInt => resultText, fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeft_ReturnsFuncLeftOutput(bool fromTask)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, string>>();

            const string resultText = "some result";
            var result = pipe.Match(fnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncRightNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipe(DefaultValue, fromTask);

            var fnError = Substitute.For<Func<PipeErrorState, Unit>>();

            Action call = () => pipe.Match(null, fnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithFuncLeftNull_ThrowsArgNullException(bool fromTask)
        {
            var pipe = CreatePipeErrorState<int>(fromTask);

            var fnT = Substitute.For<Func<PipeState<int>, Unit>>();

            Action call = () => pipe.Match(fnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithScenarioData_MapArgIsPopulated(bool fromTask)
        {
            const string someText = "some text";
            const string scenarioTitle = "Scenario title";
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "Step 1"),
                new StepOutcome(Step.And, Outcome.Fail, "Step 2")
            };

            var pipe = CreatePipe(someText, fromTask, stepOutcomes, scenarioTitle);

            pipe.Match(
                state =>
                {
                    state.Should().NotBeNull();
                    state.Value.Should().Be(someText);
                    state.Result.Should().NotBeNull();
                    state.Result.Title.Should().Be(scenarioTitle);
                    state.Result.Description.Should().Be("Scenario: Scenario title");
                    state.Result.StepResults.Should().NotBeNull();
                    state.Result.StepResults.Count.Should().Be(stepOutcomes.Count);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "Step 1", "  Given Step 1 [Passed]", Step.Given, 0);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Fail, "Step 2", "    And Step 2 [Failed]", Step.And, 1);
                    return new Unit();
                },
                error =>
                {
                    Assert.Fail("Expecting value state but was error state.");
                    return new Unit();
                });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionWithScenarioData_MapArgIsPopulated(bool fromTask)
        {
            const string someText = "some text";
            const string scenarioTitle = "Scenario title";
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "Step 1"),
                new StepOutcome(Step.And, Outcome.Fail, "Step 2")
            };

            var pipe = CreatePipe(someText, fromTask, stepOutcomes, scenarioTitle);

            pipe.Match(
                state =>
                {
                    state.Should().NotBeNull();
                    state.Value.Should().Be(someText);
                    state.Result.Should().NotBeNull();
                    state.Result.Title.Should().Be(scenarioTitle);
                    state.Result.Description.Should().Be("Scenario: Scenario title");
                    state.Result.StepResults.Should().NotBeNull();
                    state.Result.StepResults.Count.Should().Be(stepOutcomes.Count);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Pass, "Step 1", "  Given Step 1 [Passed]", Step.Given, 0);
                    state.Result.StepResults.ShouldHaveOutcomeAtIndex(Outcome.Fail, "Step 2", "    And Step 2 [Failed]", Step.And, 1);
                },
                error =>
                {
                    Assert.Fail("Expecting value state but was error state.");
                });
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithExceptionData_MapArgIsPopulated(bool fromTask)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            var pipe = CreatePipeErrorState<int>(fromTask, exInfo);

            pipe.Match(
                state =>
                {
                    Assert.Fail("Expecting error state but was value state.");
                    return new Unit();
                },
                error =>
                {
                    error.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().Be(exInfo);
                    return new Unit();
                });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Match_WithActionWithExceptionData_MapArgIsPopulated(bool fromTask)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            var pipe = CreatePipeErrorState<int>(fromTask, exInfo);

            pipe.Match(
                state =>
                {
                    Assert.Fail("Expecting error state but was value state.");
                },
                error =>
                {
                    error.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().NotBeNull();
                    error.ExceptionDispatchInfo.Should().Be(exInfo);
                });
        }
    }
}
