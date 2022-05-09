using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using static BddPipe.UnitTests.Model.PipeTests.PipeTestsHelper;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public partial class BindTests
    {
        private static Func<PipeState<string>, Pipe<int>> FnBindStringLength(string text)
        {
            var fn = Substitute.For<Func<PipeState<string>, Pipe<int>>>();
            fn(Arg.Is<PipeState<string>>(state => state.Value == text)).Returns(CreatePipe(false, text.Length));
            return fn;
        }

        private static Func<PipeState<string>, Task<Pipe<int>>> FnBindStringLengthAsync(string text)
        {
            var fn = Substitute.For<Func<PipeState<string>, Task<Pipe<int>>>>();
            fn(Arg.Is<PipeState<string>>(state => state.Value == text)).Returns(Task.FromResult(CreatePipe(false, text.Length)));
            return fn;
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Bind_PipeInSuccessState_BindArgIsPopulated(bool fromTask)
        {
            const string someText = "some text";
            const string scenarioTitle = "Scenario title";
            var stepOutcomes = new List<StepOutcome>
            {
                new StepOutcome(Step.Given, Outcome.Pass, "Step 1"),
                new StepOutcome(Step.And, Outcome.Fail, "Step 2")
            };

            var pipe = CreatePipe(fromTask, someText, stepOutcomes, scenarioTitle);

            pipe.Bind(state =>
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
                return pipe;
            });
        }

        [Test]
        public void Bind_DefaultPipeWithSyncFunc_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                var fn = Substitute.For<Func<PipeState<int>, Pipe<string>>>();
                default(Pipe<int>).Bind(fn);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void Bind_DefaultPipeWithAsyncFunc_ThrowsNotInitializedException()
        {
            Action call = () =>
            {
                var fn = Substitute.For<Func<PipeState<int>, Task<Pipe<string>>>>();
                default(Pipe<int>).Bind(fn);
            };

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Bind_FnBindStringLength_ReturnsPipe(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipe(fromTask, someText)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            fn.Received()(Arg.Is<PipeState<string>>(state => state.Value == someText));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Bind_FnBindStringLengthAsync_ReturnsPipe(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipe(fromTask, someText)
                .Bind(fn)
                .ShouldBeSuccessful(ctnT =>
                {
                    ctnT.Content.Should().Be(9);
                });

            await fn.Received()(Arg.Is<PipeState<string>>(state => state.Value == someText));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Bind_ErrorStateFnBindStringLength_HasErrorStateNoFuncRun(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLength(someText);

            CreatePipeErrorState<string>(fromTask)
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            fn.DidNotReceiveWithAnyArgs()(Arg.Any<PipeState<string>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Bind_ErrorStateFnBindStringLengthAsync_HasErrorStateNoFuncRun(bool fromTask)
        {
            const string someText = "some text";
            var fn = FnBindStringLengthAsync(someText);

            CreatePipeErrorState<string>(fromTask)
                .Bind(fn)
                .ShouldBeError(err =>
                {
                    err.Content.Should().NotBeNull();
                    err.Content.SourceException.Message.Should().Be("test error");
                });

            await fn.DidNotReceiveWithAnyArgs()(Arg.Any<PipeState<string>>());
        }
    }
}
