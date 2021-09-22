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
            pipe.MatchInternal(
                eitherCtnErrorCtnT =>
                    stepFunc.Match(
                        fnAsync => new Pipe<R>(ProcessStep(eitherCtnErrorCtnT, title, fnAsync)),
                        fnSync => new Pipe<R>(ProcessStep(eitherCtnErrorCtnT, title, fnSync))),
                taskEitherCtnErrorCtnT =>
                    stepFunc.Match(
                        fnAsync => new Pipe<R>(ProcessStep(taskEitherCtnErrorCtnT, title, fnAsync)),
                        fnSync => new Pipe<R>(ProcessStep(taskEitherCtnErrorCtnT, title, fnSync)))
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
                    var result = await step
                                    .Apply(tValue.Content)
                                    .TryRun()
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
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source, Some<Title> title, Func<T, Task<R>> step)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return await ProcessStep(sourceInstance, title, step).ConfigureAwait(false);
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessStep<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source, Some<Title> title, Func<T, R> step)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return ProcessStep(sourceInstance, title, step);
        }

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, Task<R>> step) =>
            pipe.RunStepCommon<T, R>(title, step);

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, R> step) =>
            pipe.RunStepCommon<T, R>(title, step);
    }
}
