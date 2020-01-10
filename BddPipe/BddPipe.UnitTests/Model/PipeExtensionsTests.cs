using System;
using BddPipe.Model;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class PipeExtensionsTests
    {
        private const int InitialValue = 1;
        private const int NewValue = 2;
        private const string ErrorMessage = "test message";

        private Pipe<int> GetPipeInErrorState()
        {
            var ex = new ApplicationException("test message");
            return new Ctn<Exception>(ex, None);
        }

        private Pipe<int> GetPipeInSuccessState(int withThisValue)
        {
            return new Ctn<int>(withThisValue, None);
        }

        [Test]
        public void Bind_InErrorState_RemainsInErrorStateFunctionNotInvoked()
        {
            var pipe = GetPipeInErrorState();
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

            var result = pipe.Bind(fnBind);

            fnBind.DidNotReceive()(Arg.Any<Ctn<int>>());
            result.ShouldBeError(ctnEx =>
            {
                ctnEx.Content.Should().NotBeNull();
                ctnEx.Content.GetType().Should().Be<ApplicationException>();
                ctnEx.Content.Message.Should().Be(ErrorMessage);
                ctnEx.Should().NotBeNull();
                ctnEx.ScenarioTitle.ShouldBeNone();
                ctnEx.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void Bind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

            var result = pipe.Bind(fnBind);

            fnBind.Received()(Arg.Any<Ctn<int>>());
            result.ShouldBeSuccessful(ctnValue =>
            {
                ctnValue.Content.Should().Be(NewValue);
                ctnValue.Should().NotBeNull();
                ctnValue.ScenarioTitle.ShouldBeNone();
                ctnValue.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void Bind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);
            var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var result = pipe.Bind(fnBind);

            fnBind.Received()(Arg.Any<Ctn<int>>());
            result.ShouldBeError(ctnEx =>
            {
                ctnEx.Content.Should().NotBeNull();
                ctnEx.Content.GetType().Should().Be<ApplicationException>();
                ctnEx.Content.Message.Should().Be(ErrorMessage);
                ctnEx.Should().NotBeNull();
                ctnEx.ScenarioTitle.ShouldBeNone();
                ctnEx.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void BiBind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInSuccessState(InitialValue));

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
            fnForErrorState.DidNotReceive()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeError(ctnEx =>
            {
                ctnEx.Content.Should().NotBeNull();
                ctnEx.Content.GetType().Should().Be<ApplicationException>();
                ctnEx.Content.Message.Should().Be(ErrorMessage);
                ctnEx.Should().NotBeNull();
                ctnEx.ScenarioTitle.ShouldBeNone();
                ctnEx.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void BiBind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInSuccessState(InitialValue);

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInErrorState());

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
            fnForErrorState.DidNotReceive()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeSuccessful(ctnValue =>
            {
                ctnValue.Content.Should().Be(NewValue);
                ctnValue.Should().NotBeNull();
                ctnValue.ScenarioTitle.ShouldBeNone();
                ctnValue.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void BiBind_InErrorStateFunctionBindsToErrorState_ResultIsNewErrorState()
        {
            var pipe = GetPipeInErrorState();

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInErrorState());

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
            fnForErrorState.Received()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeError(ctnEx =>
            {
                ctnEx.Content.Should().NotBeNull();
                ctnEx.Content.GetType().Should().Be<ApplicationException>();
                ctnEx.Content.Message.Should().Be(ErrorMessage);
                ctnEx.Should().NotBeNull();
                ctnEx.ScenarioTitle.ShouldBeNone();
                ctnEx.StepOutcomes.Should().BeEmpty();
            });
        }

        [Test]
        public void BiBind_InErrorStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        {
            var pipe = GetPipeInErrorState();

            var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
            fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

            var fnForErrorState = Substitute.For<Func<Ctn<Exception>, Pipe<int>>>();
            fnForErrorState(Arg.Any<Ctn<Exception>>()).Returns(GetPipeInSuccessState(NewValue));

            // act
            var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

            fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
            fnForErrorState.Received()(Arg.Any<Ctn<Exception>>());

            result.ShouldBeSuccessful(ctnValue =>
            {
                ctnValue.Content.Should().Be(NewValue);
                ctnValue.Should().NotBeNull();
                ctnValue.ScenarioTitle.ShouldBeNone();
                ctnValue.StepOutcomes.Should().BeEmpty();
            });
        }
    }
}
