using System;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerFnGivenTests
    {
        private const string AnyStringArg = "any-arg";
        private const string ScenarioText = "scenario-text";

        private Exception GetTestException() =>
            new ApplicationException("test exception message");

        private Exception GetInconclusiveException() =>
            new InconclusiveException("test inconclusive message");

        [Test]
        public void Given_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(AnyStringArg);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncRAfterScenario_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(AnyStringArg);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncRAfterScenarioThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncRAfterScenarioThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(new Unit());
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";

            var ex = GetTestException();
            Action fn = () => throw ex;

            // act
            var step = Given(title, fn);

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";

            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            // act
            var step = Given(title, fn);

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_ActionAfterScenario_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(new Unit());
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_ActionAfterScenarioThrowsException_ErrorStateSet()
        {
            const string title = "Action step";

            var ex = GetTestException();
            Action fn = () => throw ex;

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_ActionAfterScenarioThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";

            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(AnyStringArg));

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var ex = GetTestException();
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var ex = GetInconclusiveException();
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskRAfterScenario_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(AnyStringArg));

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskRAfterScenarioThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var ex = GetTestException();
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskRAfterScenarioThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var ex = GetInconclusiveException();
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(new Unit());
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var ex = GetTestException();
            var fn = Substitute.For<Func<Task>>();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var ex = GetInconclusiveException();
            var fn = Substitute.For<Func<Task>>();
            fn().Throws(ex);

            // act
            var step = Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeNone();
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskAfterScenario_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(new Unit());
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskAfterScenarioThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var ex = GetTestException();
            var fn = Substitute.For<Func<Task>>();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void Given_FuncTaskAfterScenarioThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var ex = GetInconclusiveException();
            var fn = Substitute.For<Func<Task>>();
            fn().Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }
    }
}
