using System;
using System.Runtime.ExceptionServices;

namespace BddPipe
{
    internal struct Result<A>
    {
        private readonly bool _successful;
        private readonly A _value;
        private ExceptionDispatchInfo _exceptionDispatchInfo;

        public Result(A value)
        {
            _successful = true;
            _value = value;
            _exceptionDispatchInfo = null;
        }

        public Result(ExceptionDispatchInfo e)
        {
            _successful = false;
            _exceptionDispatchInfo = e;
            _value = default;
        }

        public static implicit operator Result<A>(A value)
        {
            return new Result<A>(value);
        }

        public bool IsSuccess => _successful;

        public TResult Match<TResult>(Func<A, TResult> value, Func<ExceptionDispatchInfo, TResult> error)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            if (error == null) { throw new ArgumentNullException(nameof(error)); }

            return _successful ? value(_value) : error(_exceptionDispatchInfo);
        }
    }
}
