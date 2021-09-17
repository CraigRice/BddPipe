using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    /// <summary>
    /// BddPipe scenario runner
    /// </summary>
    public static partial class Runner
    {
        private static Pipe<R> RunStepCommon<T, R>(this Pipe<T> pipe,
                                                        Some<Title> title,
                                                        Either<Func<T, R>, Func<T, Task<R>>> stepFunc) =>
            pipe.Match(
                eitherCtnErrorCtnT =>
                    stepFunc.Match(
                        fn =>
                        {
                            var result = ProcessStep(eitherCtnErrorCtnT, title, fn);
                            return new Pipe<R>(result);
                        },
                        fnSync =>
                        {
                            var result = ProcessStep(eitherCtnErrorCtnT, title, fnSync);
                            return new Pipe<R>(result);
                        }),
                taskPipeStateT =>
                    stepFunc.Match(
                        fn =>
                        {
                            var result = ProcessStep(taskPipeStateT, title, fn);
                            return new Pipe<R>(result);
                        },
                        fnSync =>
                        {
                            var result = ProcessStep(taskPipeStateT, title, fnSync);
                            return new Pipe<R>(result);
                        })
            );

        private static Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> ProcessStep<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source, Some<Title> title, Func<T, R> step)
        {
            return source.BiBind(
                tValue =>
                    step.Apply(tValue.Content)
                        .TryRun()
                        .Match<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>>(
                            r => tValue.ToCtn(r, title.ToStepOutcome(Outcome.Pass)),
                            ex => tValue.ToCtn(ex, title.ToStepOutcome(new Some<Exception>(ex.SourceException).ToOutcome()))
                        ),
                err =>
                    err.ToCtn(err.Content, title.ToStepOutcome(Outcome.NotRun))
                );
        }

        private static Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessStep<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source, Some<Title> title, Func<T, Task<R>> step)
        {
            return source.Match(
                async tValue =>
                {
                    var result = await step.Apply(tValue.Content)
                        .TryRun();

                    return result.Match<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>>(
                        r => tValue.ToCtn(r, title.ToStepOutcome(Outcome.Pass)),
                        ex => tValue.ToCtn(ex,
                            title.ToStepOutcome(new Some<Exception>(ex.SourceException).ToOutcome()))
                    );
                },
                err =>
                {
                    Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> result =
                        err.ToCtn(err.Content, title.ToStepOutcome(Outcome.NotRun));

                    return Task.FromResult(result);
                }
            );
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessStep<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source, Some<Title> title, Func<T, Task<R>> step)
        {
            var sourceInstance = await source;
            return await ProcessStep(sourceInstance, title, step);
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessStep<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source, Some<Title> title, Func<T, R> step)
        {
            var sourceInstance = await source;
            return ProcessStep(sourceInstance, title, step);
        }

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, Task<R>> step) =>
            pipe.RunStepCommon<T, R>(title, step);

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, R> step) =>
            pipe.RunStepCommon<T, R>(title, step);
    }
}
