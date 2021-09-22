using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace BddPipe.Model
{
    internal static class PipeExtensions
    {
        public static Some<ScenarioResult> ToScenarioResult<T>(this Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctnErrorOrCtnT) =>
            ctnErrorOrCtnT.Match(
                ctnT => ctnT.ToResult(),
                ctnError => ctnError.ToResult()
            );

        public static Either<ExceptionDispatchInfo, T> ToContent<T>(this Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctnErrorOrCtnT) =>
            ctnErrorOrCtnT.Match<Either<ExceptionDispatchInfo, T>>(
                ctnT => ctnT.Content,
                ctnError => ctnError.Content
            );

        public static Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ToContainerSync<T>(this Pipe<T> pipe) =>
            pipe.MatchInternal(
                pipeData => pipeData,
                TaskFunctions.RunAndWait
            );

        public static Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> ToContainer<T>(this Pipe<T> pipe) =>
            pipe.MatchInternal(
                Task.FromResult,
                taskPipeData => taskPipeData
            );
    }
}
