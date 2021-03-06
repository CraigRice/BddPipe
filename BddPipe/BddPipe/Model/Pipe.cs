﻿using System;
using System.Threading.Tasks;

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
}
