using System;

namespace BddPipe
{
    public sealed class EitherNotInitialzedException : Exception
    {
        public EitherNotInitialzedException() : base("Either has not been initialized")
        {
        }
    }

    public struct Either<TLeft, TRight>
    {
        internal TLeft Left { get; }
        internal TRight Right { get; }

        public bool IsRight { get; }
        public bool IsLeft => !IsRight;
        private readonly bool _isInitialised;

        internal Either(TLeft left)
        {
            IsRight = false;
            Left = left;
            Right = default(TRight);
            _isInitialised = true;
        }

        internal Either(TRight right)
        {
            IsRight = true;
            Right = right;
            Left = default(TLeft);
            _isInitialised = true;
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left);
        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right);

        public TResult Match<TResult>(Func<TRight, TResult> right, Func<TLeft, TResult> left)
        {
            if (!_isInitialised)
            {
                throw new EitherNotInitialzedException();
            }
            return IsLeft ? left(this.Left) : right(this.Right);
        }

        public Unit Match(Action<TRight> payload, Action<TLeft> error)
            => Match(payload.ToFunc(), error.ToFunc());

        public override string ToString() => Match(error => $"Error({error})", payload => $"Payload({payload})");
    }
}
