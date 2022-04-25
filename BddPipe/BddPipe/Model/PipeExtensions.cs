using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace BddPipe.Model
{
    internal static class PipeExtensions
    {
        public static Some<ScenarioResult> ToScenarioResult<T>(this in Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctnErrorOrCtnT) =>
            ctnErrorOrCtnT.Match(
                ctnT => ctnT.ToResult(),
                ctnError => ctnError.ToResult()
            );

        public static Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ToContainer<T>(this in Pipe<T> pipe) =>
            pipe.MatchInternal(
                pipeData => pipeData,
                TaskFunctions.RunAndWait
            );

        public static Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> ToContainerAsync<T>(this in Pipe<T> pipe) =>
            pipe.MatchInternal(
                Task.FromResult,
                taskPipeData => taskPipeData
            );

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> ToContainerAsync<T>(this Task<Pipe<T>> taskPipe)
        {
            var pipe = await taskPipe.ConfigureAwait(false);
            return await pipe.ToContainerAsync().ConfigureAwait(false);
        }

        public static Pipe<T> AsPipe<T>(this Task<Pipe<T>> taskPipe)
        {
            var container = taskPipe.ToContainerAsync();
            return new Pipe<T>(container);
        }
    }
}
