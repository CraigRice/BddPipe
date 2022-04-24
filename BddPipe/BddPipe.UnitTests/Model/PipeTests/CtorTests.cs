using System;
using System.Runtime.ExceptionServices;
using static BddPipe.F;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;

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
            Ctn<ExceptionDispatchInfo> value = new Ctn<ExceptionDispatchInfo>(exInfo, None);
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
            Ctn<int> value = new Ctn<int>(defaultValue, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(defaultValue);
            });
        }
    }
}
