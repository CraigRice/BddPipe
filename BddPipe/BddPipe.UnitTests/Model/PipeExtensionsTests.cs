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
