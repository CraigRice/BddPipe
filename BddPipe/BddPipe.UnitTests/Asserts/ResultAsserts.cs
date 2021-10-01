using System;
using System.Runtime.ExceptionServices;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Asserts
{
    internal static class ResultAsserts
    {
        public static void ShouldBeError<T>(this Result<T> result, Action<ExceptionDispatchInfo> withError)
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();

            result.Match(val =>
            {
                Assert.Fail($"Expecting ExceptionDispatchInfo but was '{val}'");
                return new Unit();
            }, err =>
            {
                err.Should().NotBeNull();
                withError?.Invoke(err);
                return new Unit();
            });
        }

        public static void ShouldBeSuccessful<T>(this Result<T> result, Action<T> withSuccess)
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.Match(val =>
            {
                withSuccess?.Invoke(val);
                return new Unit();
            }, err =>
            {
                Assert.Fail($"Expecting T but was '{err}'");
                return new Unit();
            });
        }
    }
}
