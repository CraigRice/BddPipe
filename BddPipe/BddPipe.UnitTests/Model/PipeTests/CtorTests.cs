using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class CtorTests
    {
        [Test]
        public void CtorCtnException_WithCtnException_IsInErrorState()
        {
            var ex = new ApplicationException("test message");
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> value = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeError(error =>
            {
                error.Content.Should().Be(exInfo);
            });
        }

        [Test]
        public void CtorCtnValue_WithCtnValue_IsInValueState()
        {
            const int defaultValue = 68;
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> value = new Ctn<int>(defaultValue, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(defaultValue);
            });
        }

        [Test]
        public void CtorCtnException_WithTaskCtnException_IsInErrorState()
        {
            var ex = new ApplicationException("test message");
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> value = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var taskValue = Task.FromResult(value);

            var pipe = new Pipe<int>(taskValue);

            pipe.ShouldBeError(error =>
            {
                error.Content.Should().Be(exInfo);
            });
        }

        [Test]
        public void CtorCtnValue_WithTaskCtnValue_IsInValueState()
        {
            const int defaultValue = 68;
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> value = new Ctn<int>(defaultValue, None);
            var taskValue = Task.FromResult(value);

            var pipe = new Pipe<int>(taskValue);
            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(defaultValue);
            });
        }
    }
}
