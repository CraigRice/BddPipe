using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
        internal PipeNotInitializedException() : base("Pipe has not been initialized")
        {
        }
    }

    /// <summary>
    /// Represents either a successful outcome with the intended value or otherwise any type of Exception.
    /// <remarks>A custom Either to better represent state and make its declaration succinct compared to its full Either equivalent.</remarks>
    /// </summary>
    /// <typeparam name="T">Type of the value represented when in a successful state.</typeparam>
    public readonly struct Pipe<T>
    {
        private readonly Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> _syncResult;
        private readonly Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> _result;
        private readonly bool _isSync;
        private readonly bool _isInitialized;

        internal Pipe(in Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> result)
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

        internal TResult MatchCtnInternal<TResult>([DisallowNull] Func<Ctn<T>, TResult> containerOfValue, [DisallowNull] Func<Ctn<ExceptionDispatchInfo>, TResult> containerOfExceptionDispatchInfo)
        {
            if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
            if (containerOfExceptionDispatchInfo == null) { throw new ArgumentNullException(nameof(containerOfExceptionDispatchInfo)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync ? _syncResult : TaskFunctions.RunAndWait(_result);

            return target.Match(
                containerOfValue,
                containerOfExceptionDispatchInfo
            );
        }

        internal Unit MatchCtnInternal([DisallowNull] Action<Ctn<T>> containerOfValue, [DisallowNull] Action<Ctn<ExceptionDispatchInfo>> containerOfExceptionDispatchInfo)
        {
            if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
            if (containerOfExceptionDispatchInfo == null) { throw new ArgumentNullException(nameof(containerOfExceptionDispatchInfo)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync ? _syncResult : TaskFunctions.RunAndWait(_result);

            return target.Match(
                containerOfValue.ToFunc(),
                containerOfExceptionDispatchInfo.ToFunc()
            );
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        /// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        /// <param name="value">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="error">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns></returns>
        [return: MaybeNull]
        public TResult Match<TResult>([DisallowNull] Func<T, TResult> value, [DisallowNull] Func<ExceptionDispatchInfo, TResult> error)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (error == null) { throw new ArgumentNullException(nameof(error)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync ? _syncResult : TaskFunctions.RunAndWait(_result);

            return target.Match(
                containerOfValue => value(containerOfValue.Content),
                containerOfExceptionDispatchInfo => error(containerOfExceptionDispatchInfo.Content)
            );
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        /// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        /// <param name="value">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="error">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns></returns>
        [return: NotNull]
        public async Task<TResult> MatchAsync<TResult>([DisallowNull] Func<T, TResult> value, [DisallowNull] Func<ExceptionDispatchInfo, TResult> error)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (error == null) { throw new ArgumentNullException(nameof(error)); }
            if (!_isInitialized) { throw new PipeNotInitializedException(); }

            var target = _isSync
                ? _syncResult
                : await _result.ConfigureAwait(false);

            return target.Match(
                containerOfValue => value(containerOfValue.Content),
                containerOfExceptionDispatchInfo => error(containerOfExceptionDispatchInfo.Content)
            );
        }

        /// <summary>
        /// Performs an action based on the value based on the function implementation of each state.
        /// </summary>
        /// <param name="value">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        /// <param name="error">The function to execute if the Pipe{T} is in an error state.</param>
        /// <returns>An instance of Unit.</returns>
        public Unit Match([DisallowNull] Action<T> value, [DisallowNull] Action<ExceptionDispatchInfo> error)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (error == null) { throw new ArgumentNullException(nameof(error)); }

            return Match(value.ToFunc(), error.ToFunc());
        }
    }
}
