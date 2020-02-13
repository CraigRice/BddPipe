using System;

namespace BddPipe.Model
{
    /// <summary>
    /// Raised when a <see cref="Pipe{T}"/> instance is used - after being created as default and not initialized by normal means.
    /// </summary>
    public sealed class PipeNotInitializedException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="PipeNotInitializedException"/>
        /// </summary>
        public PipeNotInitializedException() : base("Pipe has not been initialized")
        {
        }
    }

    /// <summary>
    /// Represents either a successful outcome with the intended value or otherwise any type of Exception.
    /// <remarks>A custom Either to better represent state and make its declaration succinct compared to its full Either equivalent.</remarks>
    /// </summary>
    /// <typeparam name="T">Type of the value represented when in a successful state.</typeparam>
    public struct Pipe<T>
    {
        private readonly Ctn<Exception> _containerOfError;
        private readonly Ctn<T> _containerOfValue;

        private bool IsRight { get; }
        private bool IsLeft => !IsRight;
        private readonly bool _isInitialized;

        internal Pipe(Ctn<Exception> containerOfError)
        {
            if (containerOfError == null) throw new ArgumentNullException(nameof(containerOfError));

            IsRight = false;
            _containerOfError = containerOfError;
            _containerOfValue = default(Ctn<T>);
            _isInitialized = true;
        }

        internal Pipe(Ctn<T> containerOfValue)
        {
            if (containerOfValue == null) throw new ArgumentNullException(nameof(containerOfValue));

            IsRight = true;
            _containerOfValue = containerOfValue;
            _containerOfError = default(Ctn<Exception>);
            _isInitialized = true;
        }

        /// <summary>
        /// Lift a <see cref="Ctn{Exception}"/> into an instance of <see cref="Pipe{T}"/>
        /// </summary>
        /// <param name="containerOfError">The container instance.</param>
        public static implicit operator Pipe<T>(Ctn<Exception> containerOfError) => new Pipe<T>(containerOfError);

        /// <summary>
        /// Lift a <see cref="Ctn{T}"/> into an instance of <see cref="Pipe{T}"/>
        /// </summary>
        /// <param name="containerOfValue">The container instance.</param>
        public static implicit operator Pipe<T>(Ctn<T> containerOfValue) => new Pipe<T>(containerOfValue);

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        /// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        /// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<Ctn<T>, TResult> containerOfValue, Func<Ctn<Exception>, TResult> containerOfError)
        {
            if (!_isInitialized)
            {
                throw new PipeNotInitializedException();
            }
            return IsLeft ? containerOfError(_containerOfError) : containerOfValue(_containerOfValue);
        }

        /// <summary>
        /// Performs an action based on the value based on the function implementation of each state.
        /// </summary>
        /// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns>An instance of Unit.</returns>
        public Unit Match(Action<Ctn<T>> containerOfValue, Action<Ctn<Exception>> containerOfError)
            => Match(containerOfValue.ToFunc(), containerOfError.ToFunc());

        /// <summary>
        /// Returns a string representation of <see cref="Pipe{T}"/>
        /// </summary>
        /// <returns>The string returned indicates the contained type</returns>
        public override string ToString() => Match(payload => $"Container of ({payload})", error => $"Container of ({error})");
    }

    /// <summary>
    /// Extension methods that operate on <see cref="Pipe{T}"/>
    /// </summary>
    public static class PipeExtensions
    {
        /// <summary>
        /// Projects from one value to another and does not impact current step progress.
        /// </summary>
        /// <typeparam name="T">Current type</typeparam>
        /// <typeparam name="R">Type of the resulting value</typeparam>
        /// <param name="pipe">The <see cref="Pipe{T}"/> instance to perform this operation on.</param>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Pipe{T}"/> instance of the destination type</returns>
        public static Pipe<R> Map<T, R>(this Pipe<T> pipe, Func<T, R> map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));

            return pipe.Match<Pipe<R>>(
                ctnValue => ctnValue.Map(map),
                ctnError => ctnError
            );
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
                Func<Ctn<Exception>, Pipe<R>> bindContainerOfError
            ) => pipe.Match(bindContainerOfValue, bindContainerOfError);
    }
}
