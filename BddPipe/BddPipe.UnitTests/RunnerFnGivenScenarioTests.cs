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
    public class RunnerFnGivenScenarioTests : RunnerFnTestBase
    {
        [Test]
        public void GivenScenario_FuncScenarioRStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<Scenario, int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncScenarioRScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<Scenario, R> step";
            var fn = Substitute.For<Func<Scenario, string>>();
            fn(Arg.Any<Scenario>()).Returns(AnyStringArg);

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncScenarioR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Scenario, R> step";
            var fn = Substitute.For<Func<Scenario, string>>();
            fn(Arg.Any<Scenario>()).Returns(AnyStringArg);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Scenario, R> step";
            var fn = Substitute.For<Func<Scenario, string>>();
            var ex = GetTestException();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Scenario, R> step";
            var fn = Substitute.For<Func<Scenario, string>>();
            var ex = GetInconclusiveException();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<Scenario, Task<int>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskRScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<Scenario, Task<R>> step";
            var fn = Substitute.For<Func<Scenario, Task<string>>>();
            fn(Arg.Any<Scenario>()).Returns(AnyStringArg);

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Scenario, Task<R>> step";
            var fn = Substitute.For<Func<Scenario, Task<string>>>();
            fn(Arg.Any<Scenario>()).Returns(Task.FromResult(AnyStringArg));

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().Be(AnyStringArg);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Scenario, Task<R>> step";
            var fn = Substitute.For<Func<Scenario, Task<string>>>();
            var ex = GetTestException();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Scenario, Task<R>> step";
            var fn = Substitute.For<Func<Scenario, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncRStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncRScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(AnyStringArg);

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncR_ReceivedCallWithExpectedContext()
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
        public void GivenScenario_FuncRThrowsException_ErrorStateSet()
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
        public void GivenScenario_FuncRThrowsInconclusiveException_InconclusiveStateSet()
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
        public void GivenScenario_FuncTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<Task<int>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncTaskRScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(AnyStringArg));

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncTaskR_ReceivedCallWithExpectedContext()
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
        public void GivenScenario_FuncTaskRThrowsException_ErrorStateSet()
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
        public void GivenScenario_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
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
        public void GivenScenario_FuncScenarioTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<Scenario, Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<Scenario, Task> step";
            var fn = Substitute.For<Func<Scenario, Task>>();
            fn(Arg.Any<Scenario>()).Returns(Task.CompletedTask);

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncScenarioTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Scenario, Task> step";
            var fn = Substitute.For<Func<Scenario, Task>>();
            fn(Arg.Any<Scenario>()).Returns(Task.CompletedTask);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().BeOfType<Scenario>();
                ctn.Content.Title.Should().Be(ScenarioText);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Scenario, Task> step";
            var ex = GetTestException();
            var fn = Substitute.For<Func<Scenario, Task>>();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Fail, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncScenarioTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Scenario, Task> step";
            var ex = GetInconclusiveException();
            var fn = Substitute.For<Func<Scenario, Task>>();
            fn(Arg.Any<Scenario>()).Throws(ex);

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(ex);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Inconclusive, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Func<Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_FuncTaskScenarioNull_ThrowsArgNullException()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_FuncTask_ReceivedCallWithExpectedContext()
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
                ctn.Content.Should().BeOfType<Scenario>();
                ctn.Content.Title.Should().Be(ScenarioText);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_FuncTaskThrowsException_ErrorStateSet()
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
        public void GivenScenario_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
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

        [Test]
        public void GivenScenario_ActionScenarioStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Action<Scenario>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_ActionScenarioScenarioNull_ThrowsArgNullException()
        {
            const string title = "Action<Scenario> step";
            var fn = Substitute.For<Action<Scenario>>();

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_ActionScenario_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<Scenario> step";
            var fn = Substitute.For<Action<Scenario>>();

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()(Arg.Any<Scenario>());

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().BeOfType<Scenario>();
                ctn.Content.Title.Should().Be(ScenarioText);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_ActionScenarioThrowsException_ErrorStateSet()
        {
            const string title = "Action<Scenario> step";
            var ex = GetTestException();
            Action<Scenario> fn = scenario => throw ex;

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
        public void GivenScenario_ActionScenarioThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<Scenario> step";
            var ex = GetInconclusiveException();
            Action<Scenario> fn = scenario => throw ex;

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
        public void GivenScenario_ActionStepNull_ThrowsArgNullException()
        {
            Action call = () => Scenario(ScenarioText).Given("title", (Action)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void GivenScenario_ActionScenarioNull_ThrowsArgNullException()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            Action call = () => ((Scenario)null).Given(title, fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("scenario");
        }

        [Test]
        public void GivenScenario_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            // act
            var step = Scenario(ScenarioText).Given(title, fn);

            fn.Received()();

            step.ShouldBeSuccessful(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.Should().BeOfType<Scenario>();
                ctn.Content.Title.Should().Be(ScenarioText);
                ctn.ScenarioTitle.ShouldBeSome(scenarioText => scenarioText.Should().Be(ScenarioText));
                ctn.StepOutcomes.ShouldHaveSingleStepOutcome(Outcome.Pass, title, Step.Given);
            });
        }

        [Test]
        public void GivenScenario_ActionThrowsException_ErrorStateSet()
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
        public void GivenScenario_ActionThrowsInconclusiveException_InconclusiveStateSet()
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
    }
}
