using System;
using System.Runtime.ExceptionServices;
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
        private readonly Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> _syncResult;
        private readonly Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> _result;
        private readonly bool _isSync;
        private readonly bool _isInitialized;

        internal Pipe(Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> result)
        {
            _syncResult = result;
            _result = default;
            _isInitialized = true;
            _isSync = true;
        }

        internal Pipe(Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> result)
        {
            _syncResult = default;
            _result = result;
            _isInitialized = true;
            _isSync = false;
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        internal TResult MatchInternal<TResult>(
            Func<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>, TResult> fnSyncState,
            Func<Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>, TResult> fnAsyncState)
        {
            if (fnSyncState == null) { throw new ArgumentNullException(nameof(fnSyncState)); }
            if (fnAsyncState == null) { throw new ArgumentNullException(nameof(fnAsyncState)); }

            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            return _isSync
                ? fnSyncState(_syncResult)
                : fnAsyncState(_result);
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        /// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        /// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<Ctn<T>, TResult> containerOfValue, Func<Ctn<ExceptionDispatchInfo>, TResult> containerOfError)
        {
            if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
            if (containerOfError == null) { throw new ArgumentNullException(nameof(containerOfError)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync ? _syncResult : TaskFunctions.RunAndWait(_result);
            return target.Match(containerOfValue, containerOfError);
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        /// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        /// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns></returns>
        public async Task<TResult> MatchAsync<TResult>(Func<Ctn<T>, TResult> containerOfValue, Func<Ctn<ExceptionDispatchInfo>, TResult> containerOfError)
        {
            if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
            if (containerOfError == null) { throw new ArgumentNullException(nameof(containerOfError)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync
                ? _syncResult
                : await _result.ConfigureAwait(false);

            return target.Match(containerOfValue, containerOfError);
        }

        /// <summary>
        /// Performs an action based on the value based on the function implementation of each state.
        /// </summary>
        /// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns>An instance of Unit.</returns>
        public Unit Match(Action<Ctn<T>> containerOfValue, Action<Ctn<ExceptionDispatchInfo>> containerOfError)
        {
            if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
            if (containerOfError == null) { throw new ArgumentNullException(nameof(containerOfError)); }

            return Match(containerOfValue.ToFunc(), containerOfError.ToFunc());
        }
    }
}
