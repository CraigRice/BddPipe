using System;
using System.Runtime.ExceptionServices;

namespace BddPipe
{
    internal readonly struct Result<A>
    {
        private readonly A _value;
        private readonly ExceptionDispatchInfo _exceptionDispatchInfo;

        public Result(A value)
        {
            _exceptionDispatchInfo = null;
            _value = value;
            IsSuccess = true;
        }

        public Result(ExceptionDispatchInfo e)
        {
            _exceptionDispatchInfo = e ?? throw new ArgumentNullException(nameof(e));
            _value = default;
            IsSuccess = false;
        }

        public static implicit operator Result<A>(A value)
        {
            return new Result<A>(value);
        }

        public bool IsSuccess { get; }

        public TResult Match<TResult>(Func<A, TResult> value, Func<ExceptionDispatchInfo, TResult> error)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (error == null) { throw new ArgumentNullException(nameof(error)); }

            return IsSuccess ? value(_value) : error(_exceptionDispatchInfo);
        }
    }
}
