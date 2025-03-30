using System;

namespace BddPipe.UnitTests.Helpers;

internal static class TestExceptions
{
    public static T Raise<T>(Exception ex) => throw ex;
}
