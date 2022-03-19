using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private static Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> MapResult<T, R>(this in Result<Ctn<R>> result, Ctn<T> ctnValue)
        {
            return result.Match<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>>(
                ctnR => ctnR,
                ex => new Ctn<ExceptionDispatchInfo>(
                    ex,
                    ctnValue.StepOutcomes.WithLatestStepOutcomeAs(new Some<Exception>(ex.SourceException).ToOutcome()),
                    ctnValue.ScenarioTitle
                )
            );
        }

        private static Either<Ctn<ExceptionDispatchInfo>, Ctn<R>> ProcessMap<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, R> mapFunc)
        {
            return source.Bind(ctnValue =>
            {
                Func<Ctn<R>> mapFunction = () => ctnValue.Map(mapFunc);

                return mapFunction
                    .TryRun()
                    .MapResult(ctnValue);
            });
        }

        private static Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessMap<T, R>(
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> source,
            Func<T, Task<R>> mapFunc)
        {
            return source.BindAsync(async ctnValue =>
            {
                Func<Task<Ctn<R>>> mapFunction = () => ctnValue.MapAsync(mapFunc);

                return (await mapFunction
                    .TryRunAsync().ConfigureAwait(false))
                    .MapResult(ctnValue);
            });
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessMap<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, Task<R>> mapFunc)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return await ProcessMap(sourceInstance, mapFunc).ConfigureAwait(false);
        }

        private static async Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>> ProcessMap<T, R>(
            Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> source,
            Func<T, R> mapFunc)
        {
            var sourceInstance = await source.ConfigureAwait(false);
            return ProcessMap(sourceInstance, mapFunc);
        }

        private static Pipe<R> MapCommon<T, R>(this in Pipe<T> pipe,
                                                    in Either<Func<T, R>, Func<T, Task<R>>> mapFunc) =>
            Execute(
                pipe,
                mapFunc,
                ProcessMap,
                ProcessMap,
                ProcessMap,
                ProcessMap
            );

        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="pipe">The <see cref="Pipe{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Pipe{T}"/> instance of the destination type</returns>
        public static Pipe<R> Map<T, R>(this Pipe<T> pipe, [DisallowNull] Func<T, Task<R>> map)
        {
            if (map == null) { throw new ArgumentNullException(nameof(map)); }
            return MapCommon<T, R>(pipe, map);
        }

        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="pipe">The <see cref="Pipe{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Pipe{T}"/> instance of the destination type</returns>
        public static Pipe<R> Map<T, R>(this Pipe<T> pipe, [DisallowNull] Func<T, R> map)
        {
            if (map == null) { throw new ArgumentNullException(nameof(map)); }
            return MapCommon<T, R>(pipe, map);
        }
    }
}
