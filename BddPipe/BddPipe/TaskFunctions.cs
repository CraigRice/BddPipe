using System.Threading.Tasks;

namespace BddPipe
{
    internal static class TaskFunctions
    {
        public static R RunAndWait<R>(Task<R> fn) =>
            Task.Run(() => fn)
                .GetAwaiter()
                .GetResult();
    }
}
