using System;
using System.Runtime.ExceptionServices;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class PipeTests
    {
        private const int DefaultValue = 45;

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
            Ctn<int> value = new Ctn<int>(DefaultValue, None);
            var pipe = new Pipe<int>(value);

            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(DefaultValue);
            });
        }

        [Test]
        public void CtorCtnException_WithCtnExceptionViaImplicit_IsInErrorState()
        {
            var ex = new ApplicationException("test message");
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            Ctn<ExceptionDispatchInfo> value = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            Pipe<int> pipe = value; // lift via implicit operator

            pipe.ShouldBeError(error =>
            {
                error.Content.Should().Be(exInfo);
            });
        }

        [Test]
        public void CtorCtnValue_WithCtnValueViaImplicit_IsInValueState()
        {
            Ctn<int> value = new Ctn<int>(DefaultValue, None);
            Pipe<int> pipe = value; // lift via implicit operator

            pipe.ShouldBeSuccessful(p =>
            {
                p.Content.Should().Be(DefaultValue);
            });
        }

        [Test]
        public void CtorCtnException_NullCtn_ThrowsArgumentNullException()
        {
            Ctn<ExceptionDispatchInfo> value = null;
            Action go = () => new Pipe<int>(value);

            go.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Value cannot be null.{Environment.NewLine}Parameter name: containerOfError");
        }

        [Test]
        public void CtorCtnValue_NullCtn_ThrowsArgumentNullException()
        {
            Ctn<int> value = null;
            Action go = () => new Pipe<int>(value);

            go.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Value cannot be null.{Environment.NewLine}Parameter name: containerOfValue");
        }

        [Test]
        public void CtorCtnException_NullCtnViaImplicit_ThrowsArgumentNullException()
        {
            Ctn<ExceptionDispatchInfo> value = null;
            Action go = () =>
            {
                Pipe<int> pipe = value;
            };

            go.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Value cannot be null.{Environment.NewLine}Parameter name: containerOfError");
        }

        [Test]
        public void CtorCtnValue_NullCtnViaImplicit_ThrowsArgumentNullException()
        {
            Ctn<int> value = null;
            Action go = () =>
            {
                Pipe<int> pipe = value;
            };

            go.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Value cannot be null.{Environment.NewLine}Parameter name: containerOfValue");
        }

        [Test]
        public void Match_DefaultPipe_ThrowsNotInitializedException()
        {
            Action go = () =>
            {
                default(Pipe<int>).Match(v => v.Content, e => DefaultValue);
            };

            go.Should().ThrowExactly<PipeNotInitializedException>().WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void ToString_WithCtnDefaultValue_ReturnsCorrectString()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);
            var result = pipe.ToString();
            result.Should().Be("Container of (BddPipe.Ctn`1[System.Int32])");
        }

        [Test]
        public void ToString_WithCtnError_ReturnsCorrectString()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var result = pipe.ToString();
            result.Should().Be("Container of (BddPipe.Ctn`1[System.Runtime.ExceptionServices.ExceptionDispatchInfo])");
        }
    }
}
