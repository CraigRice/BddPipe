using System;
using System.Threading.Tasks;

namespace BddPipe
{
    internal static class TaskFunctions
    {
        public static Func<T, R> Run<T, R>(Func<T, Task<R>> fn) =>
            tArg => Task.Run(() => fn(tArg))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
    }
}
