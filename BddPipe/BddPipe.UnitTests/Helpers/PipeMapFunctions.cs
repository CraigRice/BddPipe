using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BddPipe.UnitTests.Helpers
{
    public static class PipeMapFunctions
    {
        public static Func<string, int> MapSyncRaiseEx() => value =>
        {
            var r = 5 / int.Parse("0");
            return 3;
        };

        public static Func<string, int> MapSyncRaiseInconclusiveEx() => value =>
        {
            Assert.Inconclusive("raise inconclusive");
            return 3;
        };

        public static Func<string, Task<int>> MapAsyncRaiseEx() => async value =>
        {
            await Task.Delay(10);
            var r = 5 / int.Parse("0");
            return 3;
        };

        public static Func<string, Task<int>> MapAsyncRaiseInconclusiveEx() => async value =>
        {
            await Task.Delay(10);
            Assert.Inconclusive("raise inconclusive");
            return 3;
        };
    }
}
