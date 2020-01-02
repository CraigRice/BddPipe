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
    public class RunnerFnWhenTests
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
        public void When_FuncTR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            fn(GivenValue).Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, StringValue);
        }

        [Test]
        public void When_FuncTRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            fn(GivenValue).Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, StringValue);
        }

        [Test]
        public void When_TTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, StringValue);
        }

        [Test]
        public void When_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, StringValue);
        }

        [Test]
        public void When_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            fn(GivenValue).Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, GivenValue);
        }

        [Test]
        public void When_FuncTTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, GivenValue);
        }

        [Test]
        public void When_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_ActionT_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<T> step";
            var fn = Substitute.For<Action<int>>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, GivenValue);
        }

        [Test]
        public void When_ActionTThrowsException_ErrorStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetTestException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_ActionTThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetInconclusiveException();
            Action<int> fn = i => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulStepWithValue(Step.When, GivenTitle, title, GivenValue);
        }

        [Test]
        public void When_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";
            var ex = GetTestException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            step.ShouldBeErrorStepWithException(Step.When, GivenTitle, title, ex);
        }

        [Test]
        public void When_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";
            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.When(title, fn);

            step.ShouldBeInconclusiveStepWithException(Step.When, GivenTitle, title, ex);
        }
    }
}
