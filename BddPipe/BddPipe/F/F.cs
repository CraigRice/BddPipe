﻿using System;

namespace BddPipe
{
    internal static class F
    {
        public static Func<R> Apply<T1, R>(this Func<T1, R> fn, T1 t1) => () => fn(t1);
        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> fn, T1 t1) => t2 => fn(t1, t2);
        public static Action<T1> ApplyLast<T1, T2>(this Action<T1, T2> a, T2 t2) => t1 => a(t1, t2);

        public static Func<T, Unit> ToFunc<T>(this Action<T> action)
        {
            return t =>
            {
                action(t);
                return new Unit();
            };
        }

        public static OptionNone None => OptionNone.Default;
        public static Some<T> Some<T>(T value) => new Some<T>(value);

        public static Either<L, R2> Bind<L, R, R2>(this Either<L, R> either, Func<R, Either<L, R2>> f) =>
            either.Match(r => f(r), l => l);

        public static Either<L, R2> BiBind<L, R, R2>(this Either<L, R> either, Func<R, Either<L, R2>> bindRight, Func<L, Either<L, R2>> bindLeft) =>
            either.Match(bindRight, bindLeft);

        public static Result<T> Try<T>(this Try<T> self)
        {
            try
            {
                if (self == null)
                    throw new ArgumentNullException(nameof(self));
                return self();
            }
            catch (Exception ex)
            {
                return new Result<T>(ex);
            }
        }

        public static Result<R> TryRun<R>(this Func<R> fn)
        {
            Try<R> doCall = () => fn();
            Result<R> result = doCall.Try();
            return result;
        }
    }
}
