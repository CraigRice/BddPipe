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

            return pipe.Bind(ctnValue =>
            {
                Func<Ctn<R>> mapFunction = () => ctnValue.Map(map);

                return mapFunction
                    .TryRun()
                    .Match<Pipe<R>>(
                        ctnR => ctnR,
                        ex => new Ctn<ExceptionDispatchInfo>(
                            ex,
                            ctnValue.StepOutcomes.WithLatestStepOutcomeAs(new Some<Exception>(ex.SourceException).ToOutcome()),
                            ctnValue.ScenarioTitle
                        )
                    );
            });
        }

        /// <summary>
        /// For the successful state, provide a bind function to project to a new Pipe instance. The function not invoked if in the error state already.
        /// </summary>Current
        /// <typeparam name="T">Type of the value represented when in a successful state</typeparam>
        /// <typeparam name="R">Type of the value represented when in a successful state</typeparam>
        /// <param name="pipe">Current Pipe state</param>
        /// <param name="bindContainerOfValue">A function that given a container representing a successful state, returns a new Pipe representing success or failure</param>
        /// <returns></returns>
        public static Pipe<R> Bind<T, R>(
                this Pipe<T> pipe,
                Func<Ctn<T>, Pipe<R>> bindContainerOfValue
            ) => pipe.Match(bindContainerOfValue, containerOfError => containerOfError);

        /// <summary>
        /// For each state, provide a bind function to project to a new Pipe instance. The function suited to the current state is executed.
        /// </summary>
        /// <typeparam name="T">Type of the value represented when in a successful state</typeparam>
        /// <typeparam name="R">Type of the value represented when in a successful state</typeparam>
        /// <param name="pipe">Current Pipe state</param>
        /// <param name="bindContainerOfValue">A function that given a container representing a successful state, returns a new Pipe representing success or failure</param>
        /// <param name="bindContainerOfError">A function that given a container representing an error state, returns a new Pipe representing success or failure</param>
        /// <returns></returns>
        public static Pipe<R> BiBind<T, R>(
                this Pipe<T> pipe,
                Func<Ctn<T>, Pipe<R>> bindContainerOfValue,
                Func<Ctn<ExceptionDispatchInfo>, Pipe<R>> bindContainerOfError
            ) => pipe.Match(bindContainerOfValue, bindContainerOfError);
    }
}
