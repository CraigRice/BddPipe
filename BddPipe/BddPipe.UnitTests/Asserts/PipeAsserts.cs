using BddPipe.Model;
using NUnit.Framework;
using System;
using System.Runtime.ExceptionServices;

namespace BddPipe.UnitTests.Asserts;

internal static class PipeAsserts
{
    public static void ShouldBeSuccessful<TResponse>(this Pipe<TResponse> pipe, Action<Ctn<TResponse>>? onSuccess = null)
    {
        pipe.ToContainer().Match(response =>
            {
                onSuccess?.Invoke(response);
                return new Unit();
            },
            exception =>
            {
                Assert.Fail($"Expecting a successful response of type ({typeof(TResponse)}) but was Exception {exception}");
                return new Unit();
            });
    }

    public static void ShouldBeError<TResponse>(this Pipe<TResponse> pipe, Action<Ctn<ExceptionDispatchInfo>>? onError = null)
    {
        pipe.ToContainer().Match(response =>
            {
                Assert.Fail($"Expecting an error but was successful with response of type ({typeof(TResponse)})");
                return new Unit();
            },
            exception =>
            {
                onError?.Invoke(exception);
                return new Unit();
            });
    }
}
