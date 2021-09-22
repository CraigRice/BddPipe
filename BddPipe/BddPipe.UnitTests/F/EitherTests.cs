using System;
using System.Collections.Generic;
using System.Text;

namespace BddPipe.UnitTests.F
{
    public class EitherTests
    {

        //[Test]
        //public void Bind_InErrorState_RemainsInErrorStateFunctionNotInvoked()
        //{
        //    var pipe = GetPipeInErrorState();
        //    var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

        //    var result = pipe.Bind(fnBind);

        //    fnBind.DidNotReceive()(Arg.Any<Ctn<int>>());
        //    result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        //}

        //[Test]
        //public void Bind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        //{
        //    var pipe = GetPipeInSuccessState(InitialValue);
        //    var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

        //    var result = pipe.Bind(fnBind);

        //    fnBind.Received()(Arg.Any<Ctn<int>>());
        //    result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        //}

        //[Test]
        //public void Bind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        //{
        //    var pipe = GetPipeInSuccessState(InitialValue);
        //    var fnBind = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnBind(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

        //    var result = pipe.Bind(fnBind);

        //    fnBind.Received()(Arg.Any<Ctn<int>>());
        //    result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        //}

        //[Test]
        //public void BiBind_InSuccessStateFunctionBindsToErrorState_ResultIsNewErrorState()
        //{
        //    var pipe = GetPipeInSuccessState(InitialValue);

        //    var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

        //    var fnForErrorState = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Pipe<int>>>();
        //    fnForErrorState(Arg.Any<Ctn<ExceptionDispatchInfo>>()).Returns(GetPipeInSuccessState(InitialValue));

        //    // act
        //    var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

        //    fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
        //    fnForErrorState.DidNotReceive()(Arg.Any<Ctn<ExceptionDispatchInfo>>());

        //    result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        //}

        //[Test]
        //public void BiBind_InSuccessStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        //{
        //    var pipe = GetPipeInSuccessState(InitialValue);

        //    var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(NewValue));

        //    var fnForErrorState = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Pipe<int>>>();
        //    fnForErrorState(Arg.Any<Ctn<ExceptionDispatchInfo>>()).Returns(GetPipeInErrorState());

        //    // act
        //    var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

        //    fnForSuccessState.Received()(Arg.Any<Ctn<int>>());
        //    fnForErrorState.DidNotReceive()(Arg.Any<Ctn<ExceptionDispatchInfo>>());

        //    result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        //}

        //[Test]
        //public void BiBind_InErrorStateFunctionBindsToErrorState_ResultIsNewErrorState()
        //{
        //    var pipe = GetPipeInErrorState();

        //    var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInSuccessState(InitialValue));

        //    var fnForErrorState = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Pipe<int>>>();
        //    fnForErrorState(Arg.Any<Ctn<ExceptionDispatchInfo>>()).Returns(GetPipeInErrorState());

        //    // act
        //    var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

        //    fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
        //    fnForErrorState.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());

        //    result.ShouldBeError(CtnExceptionShouldHaveErrorState);
        //}

        //[Test]
        //public void BiBind_InErrorStateFunctionBindsToSuccessState_ResultIsNewSuccessState()
        //{
        //    var pipe = GetPipeInErrorState();

        //    var fnForSuccessState = Substitute.For<Func<Ctn<int>, Pipe<int>>>();
        //    fnForSuccessState(Arg.Any<Ctn<int>>()).Returns(GetPipeInErrorState());

        //    var fnForErrorState = Substitute.For<Func<Ctn<ExceptionDispatchInfo>, Pipe<int>>>();
        //    fnForErrorState(Arg.Any<Ctn<ExceptionDispatchInfo>>()).Returns(GetPipeInSuccessState(NewValue));

        //    // act
        //    var result = pipe.BiBind(fnForSuccessState, fnForErrorState);

        //    fnForSuccessState.DidNotReceive()(Arg.Any<Ctn<int>>());
        //    fnForErrorState.Received()(Arg.Any<Ctn<ExceptionDispatchInfo>>());

        //    result.ShouldBeSuccessful(ctnValue => CtnShouldHaveValueState(ctnValue, NewValue));
        //}
    }
}
