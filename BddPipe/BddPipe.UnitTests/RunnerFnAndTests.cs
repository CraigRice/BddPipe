using System;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerFnAndTests
    {
        private const string StringValue = "string-value";
        private const string ScenarioText = "scenario-text";
        private const string GivenTitle = "given-text";
        private const int GivenValue = 12;

        private Exception GetTestException() =>
            new ApplicationException("test exception message");

        private Exception GetInconclusiveException() =>
            new InconclusiveException("test inconclusive message");

        private Pipe<int> RunnerWithGivenStep() =>
            Scenario(ScenarioText).Given(GivenTitle, () => GivenValue);

        [Test]
        public void And_FuncTRStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<int, int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncTR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            fn(GivenValue).Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void And_FuncTRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<int, Task<int>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncTTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            fn(GivenValue).Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void And_TTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncRStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void And_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<Task<int>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void And_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<int, Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncTTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            fn(GivenValue).Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void And_FuncTTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Func<Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void And_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_ActionTStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Action<int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_ActionT_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<T> step";
            var fn = Substitute.For<Action<int>>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void And_ActionTThrowsException_ErrorStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetTestException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_ActionTThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetInconclusiveException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_ActionStepNull_ThrowsArgNullException()
        {
            Action call = () => RunnerWithGivenStep().And("title", (Action)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void And_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void And_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";
            var ex = GetTestException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void And_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";
            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.And(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }
    }
}
