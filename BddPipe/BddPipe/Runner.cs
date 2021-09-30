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
            Execute(
                pipe,
                stepFunc,
                (source, fn) => ProcessStep(title, source, fn),
                (source, fn) => ProcessStep(title, source, fn),
                (source, fn) => ProcessStep(title, source, fn),
                (source, fn) => ProcessStep(title, source, fn)
            );

        private static Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> ProcessStep<T, R>(
            Some<Title> title,
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, R> step)
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
            Some<Title> title,
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, Task<R>> step)
        {
            return source.Match(
                async tValue =>
                {
                    var result = await step
                                    .Apply(tValue.Content)
                                    .TryRunAsync()
                                    .ConfigureAwait(false);

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
            Some<Title> title,
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, Task<R>> step)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return await ProcessStep(title, sourceInstance, step).ConfigureAwait(false);
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessStep<T, R>(
            Some<Title> title,
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, R> step)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return ProcessStep(title, sourceInstance, step);
        }

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, Task<R>> step) =>
            pipe.RunStepCommon<T, R>(title, step);

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, R> step) =>
            pipe.RunStepCommon<T, R>(title, step);
    }
}
