using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerFnButTests : RunnerFnTestBase
    {
        [Test]
        public void But_FuncTRStepNull_ThrowsArgNullException()
        {
            Func<int, int> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncTR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            fn(GivenValue).Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, StringValue);
        }

        [Test]
        public void But_FuncTRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTTaskRStepNull_ThrowsArgNullException()
        {
            Func<int, Task<int>> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncTTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            fn(GivenValue).Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, StringValue);
        }

        [Test]
        public void But_TTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncRStepNull_ThrowsArgNullException()
        {
            Func<int> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(StringValue);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, StringValue);
        }

        [Test]
        public void But_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTaskRStepNull_ThrowsArgNullException()
        {
            Func<Task<int>> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(StringValue));

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, StringValue);
        }

        [Test]
        public void But_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTTaskStepNull_ThrowsArgNullException()
        {
            Func<int, Task> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncTTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            fn(GivenValue).Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, GivenValue);
        }

        [Test]
        public void But_FuncTTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTaskStepNull_ThrowsArgNullException()
        {
            Func<Task> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, GivenValue);
        }

        [Test]
        public void But_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_ActionTStepNull_ThrowsArgNullException()
        {
            Func<Action<int>> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_ActionT_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<T> step";
            var fn = Substitute.For<Action<int>>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, GivenValue);
        }

        [Test]
        public void But_ActionTThrowsException_ErrorStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetTestException();
            Action<int> fn = _ => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_ActionTThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetInconclusiveException();
            Action<int> fn = _ => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_ActionStepNull_ThrowsArgNullException()
        {
            Func<Action> fn = null!;

            Action call = () => RunnerWithGivenStep().But("title", fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void But_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.But, GivenTitle, title, GivenValue);
        }

        [Test]
        public void But_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";
            var ex = GetTestException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.But, GivenTitle, title, ex);
        }

        [Test]
        public void But_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";
            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            var runner = RunnerWithGivenStep();

            // act
            var step = runner.But(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.But, GivenTitle, title, ex);
        }
    }
}
