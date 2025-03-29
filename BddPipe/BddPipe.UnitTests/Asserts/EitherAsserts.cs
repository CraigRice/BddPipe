using NUnit.Framework;
using System;

namespace BddPipe.UnitTests.Asserts;

internal static class EitherAsserts
{
    public static void ShouldBeRight<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TRight>? onRight = null)
    {
        either.Match(right =>
            {
                onRight?.Invoke(right);
                return new Unit();
            },
            left =>
            {
                Assert.Fail($"Expecting a response of type ({typeof(TRight)}) but was of type ({typeof(TLeft)}): {left}");
                return new Unit();
            });
    }

    public static void ShouldBeLeft<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft>? onLeft = null)
    {
        either.Match(right =>
            {
                Assert.Fail($"Expecting a response of type ({typeof(TLeft)}) but was of type ({typeof(TRight)}): {right}");
                return new Unit();
            },
            left =>
            {
                onLeft?.Invoke(left);
                return new Unit();
            });
    }
}
