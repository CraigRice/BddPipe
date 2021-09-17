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
        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="pipe">The <see cref="Pipe{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Pipe{T}"/> instance of the destination type</returns>
        public static Pipe<R> Map<T, R>(this Pipe<T> pipe, Func<T, Task<R>> map) =>
            pipe.Map(TaskFunctions.Run(map));

        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="pipe">The <see cref="Pipe{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Pipe{T}"/> instance of the destination type</returns>
        public static Pipe<R> Map<T, R>(this Pipe<T> pipe, Func<T, R> map)
        {
            if (map == null) { throw new ArgumentNullException(nameof(map)); }

            return new Pipe<R>(new Either<Ctn<ExceptionDispatchInfo>, Ctn<R>>());
            //return pipe.Bind(ctnValue =>
            //{
            //    Func<Ctn<R>> mapFunction = () => ctnValue.Map(map);

            //    return mapFunction
            //        .TryRun()
            //        .Match<Pipe<R>>(
            //            ctnR => ctnR,
            //            ex => new Ctn<ExceptionDispatchInfo>(
            //                ex,
            //                ctnValue.StepOutcomes.WithLatestStepOutcomeAs(new Some<Exception>(ex.SourceException).ToOutcome()),
            //                ctnValue.ScenarioTitle
            //            )
            //        );
            //});
        }
    }
}
