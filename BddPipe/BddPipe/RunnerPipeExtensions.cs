using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    /// <summary>
    /// Extension methods that operate on <see cref="Pipe{T}"/>
    /// </summary>
    public static partial class Runner
    {
        private delegate Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> SyncRunAsyncToAsync<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, Task<R>> fn);

        private delegate Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> SyncRunSyncToSync<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, R> fn);

        private delegate Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> AsyncRunAsyncToAsync<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, Task<R>> fn);

        private delegate Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> AsyncRunSyncToAsync<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, R> fn);

        private static Pipe<R> Execute<T, R>(this Pipe<T> pipe,
            Either<Func<T, R>, Func<T, Task<R>>> fn,
            SyncRunAsyncToAsync<T, R> syncRunAsyncToAsync,
            SyncRunSyncToSync<T, R> syncRunSyncToSync,
            AsyncRunAsyncToAsync<T,R> asyncRunAsyncToAsync,
            AsyncRunSyncToAsync<T,R> asyncRunSyncToAsync
            ) =>
            pipe.MatchInternal(
                eitherCtnErrorCtnT =>
                    fn.Match(
                        fnAsync => new Pipe<R>(syncRunAsyncToAsync(eitherCtnErrorCtnT, fnAsync)),
                        fnSync => new Pipe<R>(syncRunSyncToSync(eitherCtnErrorCtnT, fnSync))),
                taskEitherCtnErrorCtnT =>
                    fn.Match(
                        fnAsync => new Pipe<R>(asyncRunAsyncToAsync(taskEitherCtnErrorCtnT, fnAsync)),
                        fnSync => new Pipe<R>(asyncRunSyncToAsync(taskEitherCtnErrorCtnT, fnSync)))
            );
    }
}
