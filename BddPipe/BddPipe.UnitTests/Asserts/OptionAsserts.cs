using NUnit.Framework;
using System;

namespace BddPipe.UnitTests.Asserts;

internal static class OptionAsserts
{
    public static void ShouldBeSome<T>(this Option<T> option, Action<T>? onSuccess = null)
    {
        option.Match(t =>
            {
                onSuccess?.Invoke(t);
                return new Unit();
            },
            () =>
            {
                Assert.Fail($"Expecting Some({typeof(T)}) but was None");
                return new Unit();
            });
    }

    public static void ShouldBeNone<T>(this Option<T> option, Action? onNone = null)
    {
        option.Match(response =>
            {
                Assert.Fail($"Expecting None but was Some({typeof(T)})");
                return new Unit();
            },
            () =>
            {
                onNone?.Invoke();
                return new Unit();
            });
    }
}
