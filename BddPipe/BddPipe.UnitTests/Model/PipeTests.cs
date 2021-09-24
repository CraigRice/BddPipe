using System;
using System.Runtime.ExceptionServices;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
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

            go.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");
        }

        [Test]
        public void CtorCtnValue_NullCtn_ThrowsArgumentNullException()
        {
            Ctn<int> value = null;
            Action go = () => new Pipe<int>(value);

            go.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");
        }

        [Test]
        public void CtorCtnException_NullCtnViaImplicit_ThrowsArgumentNullException()
        {
            Ctn<ExceptionDispatchInfo> value = null;
            Action go = () =>
            {
                Pipe<int> pipe = value;
            };

            go.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");
        }

        [Test]
        public void CtorCtnValue_NullCtnViaImplicit_ThrowsArgumentNullException()
        {
            Ctn<int> value = null;
            Action go = () =>
            {
                Pipe<int> pipe = value;
            };

            go.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");
        }

        [Test]
        public void Match_DefaultPipe_ThrowsNotInitializedException()
        {
            Action go = () =>
            {
                default(Pipe<int>).Match(v => v.Content, e => DefaultValue);
            };

            go.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void Match_WithActionCtnDefaultValue_CallsActionCtnT()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();
            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithActionCtnError_CallsActionCtnError()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();
            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [Test]
        public void Match_WithActionCtnDefaultValueNull_ThrowsArgNullException()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);

            var fnCtnError = Substitute.For<Action<Ctn<ExceptionDispatchInfo>>>();

            Action call = () => pipe.Match(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithActionCtnErrorNull_ThrowsArgNullException()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Action<Ctn<int>>>();

            Action call = () => pipe.Match(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");

            fnCtnT.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnDefaultValue_CallsFuncCtnT()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.Received()(Arg.Any<Ctn<int>>());
            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnError_CallsFuncCtnError()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();
            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            pipe.Match(fnCtnT, fnCtnError);

            fnCtnT.DidNotReceive();
            fnCtnError.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());
        }

        [Test]
        public void Match_WithFuncCtnDefaultValue_ReturnsFuncOutput()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, string>>();

            const string resultText = "some result";
            var result = pipe.Match(ctnInt => resultText, fnCtnError);

            result.Should().Be(resultText);

            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnError_ReturnsFuncOutput()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, string>>();

            const string resultText = "some result";
            var result = pipe.Match(fnCtnT, ctnError => resultText);

            result.Should().Be(resultText);

            fnCtnT.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnDefaultValueNull_ThrowsArgNullException()
        {
            Pipe<int> pipe = new Ctn<int>(DefaultValue, None);

            var fnCtnError = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Unit>>();

            Action call = () => pipe.Match(null, fnCtnError);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfValue");

            fnCtnError.DidNotReceive();
        }

        [Test]
        public void Match_WithFuncCtnErrorNull_ThrowsArgNullException()
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Pipe<int> pipe = new Ctn<ExceptionDispatchInfo>(exInfo, None);

            var fnCtnT = Substitute.For<Func<Ctn<int>, Unit>>();

            Action call = () => pipe.Match(fnCtnT, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("containerOfError");

            fnCtnT.DidNotReceive();
        }
    }
}
