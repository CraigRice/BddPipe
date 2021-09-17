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

    internal sealed class PipeState<T>
    {
        private readonly Ctn<ExceptionDispatchInfo> _ctnError;
        private readonly Ctn<T> _ctnPayload;
        private readonly bool _isPayload;

        public PipeState(Ctn<ExceptionDispatchInfo> ctnError)
        {
            _ctnError = ctnError;
            _isPayload = false;
        }

        public PipeState(Ctn<T> ctnPayload)
        {
            _ctnPayload = ctnPayload;
            _isPayload = true;
        }

        public TResult Match<TResult>(Func<Ctn<T>, TResult> right, Func<Ctn<ExceptionDispatchInfo>, TResult> left)
        {
            if (right == null) { throw new ArgumentNullException(nameof(right)); }
            if (left == null) { throw new ArgumentNullException(nameof(left)); }

            return _isPayload
                ? right(_ctnPayload)
                : left(_ctnError);
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
        private readonly bool _isInitialised;

        internal Pipe(Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> result)
        {
            _syncResult = result;
            _result = default;
            _isInitialised = true;
            _isSync = true;
        }

        internal Pipe(Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> result)
        {
            _syncResult = default;
            _result = result;
            _isInitialised = true;
            _isSync = false;
        }

        /// <summary>
        /// Returns the value based on the function implementation of each state.
        /// </summary>
        public TResult Match<TResult>(Func<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>, TResult> right,
            Func<Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>, TResult> left)
        {
            if (right == null) { throw new ArgumentNullException(nameof(right)); }
            if (left == null) { throw new ArgumentNullException(nameof(left)); }

            if (!_isInitialised) { throw new PipeNotInitializedException(); }

            return _isSync
                ? right(_syncResult)
                : left(_result);
        }

        ///// <summary>
        ///// Lift a <see cref="Ctn{Exception}"/> into an instance of <see cref="Pipe{T}"/>
        ///// </summary>
        ///// <param name="containerOfError">The container instance.</param>
        //public static implicit operator Pipe<T>(Ctn<ExceptionDispatchInfo> containerOfError) => new Pipe<T>(containerOfError);

        ///// <summary>
        ///// Lift a <see cref="Ctn{T}"/> into an instance of <see cref="Pipe{T}"/>
        ///// </summary>
        ///// <param name="containerOfValue">The container instance.</param>
        //public static implicit operator Pipe<T>(Ctn<T> containerOfValue) => new Pipe<T>(containerOfValue);

        ///// <summary>
        ///// Returns the value based on the function implementation of each state.
        ///// </summary>
        ///// <typeparam name="TResult">The target return result type to be returned by both supplied functions.</typeparam>
        ///// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        ///// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        ///// <returns></returns>
        //public TResult Match<TResult>(Func<Ctn<T>, TResult> containerOfValue, Func<Ctn<ExceptionDispatchInfo>, TResult> containerOfError)
        //{
        //    if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
        //    if (containerOfError == null) { throw new ArgumentNullException(nameof(containerOfError)); }

        //    if (!_isInitialized)
        //    {
        //        throw new PipeNotInitializedException();
        //    }

        //    return IsLeft ? containerOfError(_containerOfError) : containerOfValue(_containerOfValue);
        //}

        ///// <summary>
        ///// Performs an action based on the value based on the function implementation of each state.
        ///// </summary>
        ///// <param name="containerOfValue">The function to execute if the Pipe{T} is in a success state with the desired value.</param>
        ///// <param name="containerOfError">The function to execute if the Pipe{T} is in an error state.</param>
        ///// <returns>An instance of Unit.</returns>
        //public Unit Match(Action<Ctn<T>> containerOfValue, Action<Ctn<ExceptionDispatchInfo>> containerOfError)
        //{
        //    if (containerOfValue == null) { throw new ArgumentNullException(nameof(containerOfValue)); }
        //    if (containerOfError == null) { throw new ArgumentNullException(nameof(containerOfError)); }

        //    return Match(containerOfValue.ToFunc(), containerOfError.ToFunc());
        //}

        /// <summary>
        /// Returns a string representation of <see cref="Pipe{T}"/>
        /// </summary>
        /// <returns>The string returned indicates the contained type</returns>
        // public override string ToString() => Match(payload => $"Container of ({payload})", error => $"Container of ({error})");
    }

    internal static class PipeExtensions
    {
        public static Some<ScenarioResult> ToScenarioResult<T>(this Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctnErrorOrCtnT) =>
            ctnErrorOrCtnT.Match(
                ctnT => ctnT.ToResult(),
                ctnError => ctnError.ToResult()
            );

        public static Either<ExceptionDispatchInfo, T> ToContent<T>(this Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctnErrorOrCtnT) =>
            ctnErrorOrCtnT.Match<Either<ExceptionDispatchInfo, T>>(
                ctnT => ctnT.Content,
                ctnError => ctnError.Content
            );

        public static Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ToContainerSync<T>(this Pipe<T> pipe) =>
            pipe.Match(
                pipeData => pipeData,
                TaskFunctions.RunAndWait
            );

        public static Task<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>> ToContainer<T>(this Pipe<T> pipe) =>
            pipe.Match(
                Task.FromResult,
                taskPipeData => taskPipeData
            );
    }
}
