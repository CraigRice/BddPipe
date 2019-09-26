using System;

namespace BddPipe
{
    internal struct Result<A>
    {
        internal readonly bool Successful;
        internal readonly A Value;
        internal Exception Exception;

        public Result(A value)
        {
            Successful = true;
            Value = value;
            Exception = (Exception)null;
        }

        public Result(Exception e)
        {
            Successful = false;
            Exception = e;
            Value = default(A);
        }

        public static implicit operator Result<A>(A value)
        {
            return new Result<A>(value);
        }

        public bool IsFaulted => !Successful;

        public bool IsSuccess => Successful;

        public TResult Match<TResult>(Func<A, TResult> value, Func<Exception, TResult> error) =>
            Successful ? value(Value) : error(Exception);
    }
}
