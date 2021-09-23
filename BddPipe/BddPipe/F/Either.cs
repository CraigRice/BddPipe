using System;

namespace BddPipe
{
    internal sealed class EitherNotInitialzedException : Exception
    {
        public EitherNotInitialzedException() : base("Either has not been initialized")
        {
        }
    }

    internal readonly struct Either<TLeft, TRight>
    {
        private TLeft Left { get; }
        private TRight Right { get; }

        public bool IsRight { get; }
        public bool IsLeft => !IsRight;
        private readonly bool _isInitialized;

        public Either(TLeft left)
        {
            if (left == null) { throw new ArgumentNullException(nameof(left)); }

            IsRight = false;
            Left = left;
            Right = default;
            _isInitialized = true;
        }

        public Either(TRight right)
        {
            if (right == null) { throw new ArgumentNullException(nameof(right)); }

            IsRight = true;
            Right = right;
            Left = default;
            _isInitialized = true;
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }

        public TResult Match<TResult>(Func<TRight, TResult> right, Func<TLeft, TResult> left)
        {
            if (right == null) { throw new ArgumentNullException(nameof(right)); }
            if (left == null) { throw new ArgumentNullException(nameof(left)); }
            if (!_isInitialized) { throw new EitherNotInitialzedException(); }

            return IsLeft
                ? left(Left)
                : right(Right);
        }

        public Either<TLeft, R> BiBind<R>(
            Func<TRight, Either<TLeft, R>> right,
            Func<TLeft, Either<TLeft, R>> left
        )
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (left == null) throw new ArgumentNullException(nameof(left));

            return Match(right, left);
        }

        public override string ToString()
        {
            return Match(right => $"right({right})", left => $"left({left})");
        }
    }
}
