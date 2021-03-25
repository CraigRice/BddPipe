using System;
using System.Runtime.ExceptionServices;

namespace BddPipe
{
    internal struct Result<A>
    {
        internal readonly bool Successful;
        internal readonly A Value;
        internal ExceptionDispatchInfo ExceptionDispatchInfo;

        public Result(A value)
        {
            Successful = true;
            Value = value;
            ExceptionDispatchInfo = null;
        }

        public Result(ExceptionDispatchInfo e)
        {
            Successful = false;
            ExceptionDispatchInfo = e;
            Value = default(A);
        }

        public static implicit operator Result<A>(A value)
        {
            return new Result<A>(value);
        }

        public bool IsFaulted => !Successful;

        public bool IsSuccess => Successful;

        public TResult Match<TResult>(Func<A, TResult> value, Func<ExceptionDispatchInfo, TResult> error) =>
            Successful ? value(Value) : error(ExceptionDispatchInfo);
    }
}
