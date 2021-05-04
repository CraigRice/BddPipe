using System;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class RecipeTests : RunnerFnTestBase
    {
        private Recipe<int, string> RecipeOfGivenStep()
        {
            var pipe = RunnerWithGivenStep();
            return new Recipe<int, string>(pipe, Step.And);
        }

        [Test]
        public void Map_MapFnSupplied_DoesNotThrow()
        {
            Action call = () => RecipeOfGivenStep().Map(intValue => Guid.NewGuid());
            call.Should().NotThrow();
        }

        [Test]
        public void Map_MapFnNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Map((Func<int, Guid>) null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }

        [Test]
        public void Map_MapFnThrows_DoesNotThrow()
        {
            // map won't raise the exception, the pipe will change to failed on its current step
            Func<int, Guid> mapFn = intValue => throw GetTestException();
            Action call = () => RecipeOfGivenStep().Map(mapFn);
            call.Should().NotThrow();
        }

        [Test]
        public void Step_AfterMap_FunctionRunsWithMappedValue()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<Guid, string>>();
            var newValue = Guid.NewGuid();

            fn(newValue).Returns(StringValue);

            // act
            var step = RecipeOfGivenStep()
                .Map(intValue => newValue)
                .Step(title, fn);

            fn.Received()(newValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void Step_AfterMapThrows_ResultHasFailedStep()
        {
            const string title = "not run step";
            var expectedException = GetTestException();
            Func<int, Guid> mapFn = intValue => throw expectedException;

            // act
            var step = RecipeOfGivenStep()
                .Map(mapFn)
                .Step(title, g => AnyStringArg);

            step.ShouldBeError(ctn =>
            {
                ctn.Should().NotBeNull();
                ctn.Content.SourceException.Should().Be(expectedException);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.Fail, GivenTitle, Step.Given, 0);
                ctn.StepOutcomes.ShouldHaveStepOutcomeAtIndex(Outcome.NotRun, title, Step.And, 1);
                ctn.StepOutcomes.Count.Should().Be(2);
            });
        }

        [Test]
        public void Step_FuncTRStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<int, string>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncTR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            fn(GivenValue).Returns(StringValue);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void Step_FuncTRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, R> step";
            var fn = Substitute.For<Func<int, string>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<int, Task<string>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncTTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            fn(GivenValue).Returns(Task.FromResult(StringValue));

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void Step_TTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task<R>> step";
            var fn = Substitute.For<Func<int, Task<string>>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncRStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<string>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            fn().Returns(StringValue);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void Step_FuncRThrowsException_ErrorStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<R> step";
            var fn = Substitute.For<Func<string>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTaskRStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<Task<string>>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncTaskR_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            fn().Returns(Task.FromResult(StringValue));

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, StringValue);
        }

        [Test]
        public void Step_FuncTaskRThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTaskRThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task<R>> step";
            var fn = Substitute.For<Func<Task<string>>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<int, Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncTTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            fn(GivenValue).Returns(Task.CompletedTask);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Step_FuncTTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetTestException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<T, Task> step";
            var fn = Substitute.For<Func<int, Task>>();
            var ex = GetInconclusiveException();
            fn(GivenValue).Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTaskStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Func<Task>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_FuncTask_ReceivedCallWithExpectedContext()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            fn().Returns(Task.CompletedTask);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Step_FuncTaskThrowsException_ErrorStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetTestException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_FuncTaskThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Func<Task> step";
            var fn = Substitute.For<Func<Task>>();
            var ex = GetInconclusiveException();
            fn().Throws(ex);

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_ActionTStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Action<int>)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_ActionT_ReceivedCallWithExpectedContext()
        {
            const string title = "Action<T> step";
            var fn = Substitute.For<Action<int>>();

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()(GivenValue);
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Step_ActionTThrowsException_ErrorStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetTestException();
            Action<int> fn = i => throw ex;

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_ActionTThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action<T> step";
            var ex = GetInconclusiveException();
            Action<int> fn = i => throw ex;

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_ActionStepNull_ThrowsArgNullException()
        {
            Action call = () => RecipeOfGivenStep().Step("title", (Action)null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("step");
        }

        [Test]
        public void Step_Action_ReceivedCallWithExpectedContext()
        {
            const string title = "Action step";
            var fn = Substitute.For<Action>();

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            fn.Received()();
            step.ShouldBeSuccessfulSecondStepWithValue(Step.And, GivenTitle, title, GivenValue);
        }

        [Test]
        public void Step_ActionThrowsException_ErrorStateSet()
        {
            const string title = "Action step";
            var ex = GetTestException();
            Action fn = () => throw ex;

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            step.ShouldBeErrorSecondStepWithException(Step.And, GivenTitle, title, ex);
        }

        [Test]
        public void Step_ActionThrowsInconclusiveException_InconclusiveStateSet()
        {
            const string title = "Action step";
            var ex = GetInconclusiveException();
            Action fn = () => throw ex;

            var runner = RecipeOfGivenStep();

            // act
            var step = runner.Step(title, fn);

            step.ShouldBeInconclusiveSecondStepWithException(Step.And, GivenTitle, title, ex);
        }
    }
}
