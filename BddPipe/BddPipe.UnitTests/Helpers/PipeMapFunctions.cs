using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BddPipe.UnitTests.Helpers;

internal static class PipeMapFunctions
{
    public static Func<string, int> MapSyncRaiseEx() => _ =>
    {
        var r = 5 / int.Parse("0");
        return r;
    };

    public static Func<string, int> MapSyncRaiseInconclusiveEx() => _ =>
    {
        Assert.Inconclusive("raise inconclusive");
        return 3;
    };

    public static Func<string, Task<int>> MapAsyncRaiseEx() => async _ =>
    {
        await Task.Delay(10);
        var r = 5 / int.Parse("0");
        return r;
    };

    public static Func<string, Task<int>> MapAsyncRaiseInconclusiveEx() => async _ =>
    {
        await Task.Delay(10);
        Assert.Inconclusive("raise inconclusive");
        return 3;
    };
}
