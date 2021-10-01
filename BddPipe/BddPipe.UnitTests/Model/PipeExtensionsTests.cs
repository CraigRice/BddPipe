using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class PipeExtensionsTests
    {
        private const int DefaultValue = 45;

        [Test]
        public void ToScenarioResult_CreateWithTHavingEmptyResult_ResultIsCorrect()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;

            var scenarioResult = pipeContent.ToScenarioResult();

            var scenario = scenarioResult.Value;
            scenario.Title.Should().BeNull();
            scenario.StepResults.Should().NotBeNull();
            scenario.StepResults.Should().BeEmpty();
            scenario.Description.Should().Be("Scenario:");
        }

        [Test]
        public void ToScenarioResult_CreateWithErrorHavingEmptyResult_ResultIsCorrect()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;

            var scenarioResult = pipeContent.ToScenarioResult();

            var scenario = scenarioResult.Value;
            scenario.Title.Should().BeNull();
            scenario.StepResults.Should().NotBeNull();
            scenario.StepResults.Should().BeEmpty();
            scenario.Description.Should().Be("Scenario:");
        }

        [Test]
        public void ToScenarioResult_CreateWithTHavingResult_ResultIsCorrect()
        {
            const string title = "the scenario title";
            var ctnT = new Ctn<int>(DefaultValue, title);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;

            var scenarioResult = pipeContent.ToScenarioResult();

            var scenario = scenarioResult.Value;
            scenario.Title.Should().Be(title);
            scenario.StepResults.Should().NotBeNull();
            scenario.StepResults.Should().BeEmpty();
            scenario.Description.Should().Be($"Scenario: {title}");
        }

        [Test]
        public void ToScenarioResult_CreateWithErrorHavingResult_ResultIsCorrect()
        {
            const string title = "the scenario title";
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, title);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;

            var scenarioResult = pipeContent.ToScenarioResult();

            var scenario = scenarioResult.Value;
            scenario.Title.Should().Be(title);
            scenario.StepResults.Should().NotBeNull();
            scenario.StepResults.Should().BeEmpty();
            scenario.Description.Should().Be($"Scenario: {title}");
        }

        [Test]
        public void ToContent_CreateWithT_EitherHasCorrectStateAndContent()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;

            var container = pipeContent.ToContent();

            container.ShouldBeRight(t =>
            {
                t.Should().Be(DefaultValue);
            });
        }

        [Test]
        public void ToContent_CreateWithError_EitherHasCorrectStateAndContent()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;

            var container = pipeContent.ToContent();

            container.ShouldBeLeft(exceptionDispatchInfo =>
            {
                exceptionDispatchInfo.Should().BeSameAs(exInfo);
            });
        }

        [Test]
        public void ToContainer_CreateWithAsyncT_EitherHasCorrectStateAndContent()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;
            var pipe = new Pipe<int>(Task.FromResult(pipeContent));

            var container = pipe.ToContainer();

            container.ShouldBeRight(ctnTResult =>
            {
                ctnTResult.Should().BeSameAs(ctnT);
            });
        }

        [Test]
        public void ToContainer_CreateWithSyncT_EitherHasCorrectStateAndContent()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;
            var pipe = new Pipe<int>(pipeContent);

            var container = pipe.ToContainer();

            container.ShouldBeRight(ctnTResult =>
            {
                ctnTResult.Should().BeSameAs(ctnT);
            });
        }

        [Test]
        public void ToContainer_CreateWithSyncError_EitherHasCorrectStateAndContent()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;
            var pipe = new Pipe<int>(pipeContent);

            var container = pipe.ToContainer();

            container.ShouldBeLeft(ctnErrorResult =>
            {
                ctnErrorResult.Should().BeSameAs(ctnError);
            });
        }

        [Test]
        public void ToContainer_CreateWithAsyncError_EitherHasCorrectStateAndContent()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;
            var pipe = new Pipe<int>(Task.FromResult(pipeContent));

            var container = pipe.ToContainer();

            container.ShouldBeLeft(ctnErrorResult =>
            {
                ctnErrorResult.Should().BeSameAs(ctnError);
            });
        }

        [Test]
        public async Task ToContainerAsync_CreateWithAsyncT_EitherHasCorrectStateAndContent()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;
            var pipe = new Pipe<int>(Task.FromResult(pipeContent));

            var container = await pipe.ToContainerAsync();

            container.ShouldBeRight(ctnTResult =>
            {
                ctnTResult.Should().BeSameAs(ctnT);
            });
        }

        [Test]
        public async Task ToContainerAsync_CreateWithSyncT_EitherHasCorrectStateAndContent()
        {
            var ctnT = new Ctn<int>(DefaultValue, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnT;
            var pipe = new Pipe<int>(pipeContent);

            var container = await pipe.ToContainerAsync();

            container.ShouldBeRight(ctnTResult =>
            {
                ctnTResult.Should().BeSameAs(ctnT);
            });
        }

        [Test]
        public async Task ToContainerAsync_CreateWithSyncError_EitherHasCorrectStateAndContent()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;
            var pipe = new Pipe<int>(pipeContent);

            var container = await pipe.ToContainerAsync();

            container.ShouldBeLeft(ctnErrorResult =>
            {
                ctnErrorResult.Should().BeSameAs(ctnError);
            });
        }

        [Test]
        public async Task ToContainerAsync_CreateWithAsyncError_EitherHasCorrectStateAndContent()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            var ctnError = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeContent = ctnError;
            var pipe = new Pipe<int>(Task.FromResult(pipeContent));

            var container = await pipe.ToContainerAsync();

            container.ShouldBeLeft(ctnErrorResult =>
            {
                ctnErrorResult.Should().BeSameAs(ctnError);
            });
        }
    }
}
