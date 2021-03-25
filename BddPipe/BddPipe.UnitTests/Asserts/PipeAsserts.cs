using System;
using System.Runtime.ExceptionServices;
using BddPipe.Model;
using NUnit.Framework;

namespace BddPipe.UnitTests.Asserts
{
    internal static class PipeAsserts
    {
        public static void ShouldBeSuccessful<TResponse>(this Pipe<TResponse> pipe, Action<Ctn<TResponse>> onSuccess = null)
        {
            pipe.Match(response =>
                {
                    onSuccess?.Invoke(response);
                },
                exception =>
                {
                    Assert.Fail($"Expecting a successful response of type ({typeof(TResponse)}) but was Exception {exception}");
                });
        }

        public static void ShouldBeError<TResponse>(this Pipe<TResponse> pipe, Action<Ctn<ExceptionDispatchInfo>> onError = null)
        {
            pipe.Match(response =>
                {
                    Assert.Fail($"Expecting an error but was successful with response of type ({typeof(TResponse)})");
                },
                exception =>
                {
                    onError?.Invoke(exception);
                });
        }
    }
}
