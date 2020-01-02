using System;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerFnThenTests
    {
        private const string StringValue = "string-value";
        private const string ScenarioText = "scenario-text";
        private const string GivenTitle = "given-text";
        private const int GivenValue = 12;

        private Exception GetTestException() =>
            new ApplicationException("test exception message");

        private Exception GetInconclusiveException() =>
            new InconclusiveException("test inconclusive message");

        private Either<Ctn<Exception>, Ctn<int>> RunnerWithGivenStep() =>
            Scenario(ScenarioText).Given(GivenTitle, () => GivenValue);

        [Test]
        public void Then_FuncTR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            fn(GivenValue).Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, StringValue);
        }

        [Test]
        public void Then_FuncTRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            fn(GivenValue).Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, StringValue);
        }

        [Test]
        public void Then_TTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, StringValue);
        }

        [Test]
        public void Then_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, StringValue);
        }

        [Test]
        public void Then_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            fn(GivenValue).Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Then_FuncTTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Then_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_ActionT_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<T> step";
            var fn = Substitute.For<Action<int>>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Then_ActionTThrowsException_ErrorStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetTestException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_ActionTThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetInconclusiveException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.Then, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Then_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";
            var ex = GetTestException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            step.ShouldBeErrorStepWithException(Step.Then, GivenTitle, title, ex);
        }

        [Test]
        public void Then_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";
            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.Then(title, fn);

            step.ShouldBeInconclusiveStepWithException(Step.Then, GivenTitle, title, ex);
        }
    }
}
