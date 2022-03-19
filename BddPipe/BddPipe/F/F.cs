using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace BddPipe
{
    internal static class F
    {
        public static Func<R> Apply<T1, R>(this Func<T1, R> fn, T1 t1) =>
            () => fn(t1);

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

        public static Result<T> Try<T>(this Try<T> fn)
        {
            if (fn == null) { throw new ArgumentNullException(nameof(fn)); }

            try
            {
                return fn();
            }
            catch (Exception ex)
            {
                var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                return new Result<T>(exceptionDispatchInfo);
            }
        }

        public static async Task<Result<T>> TryAsync<T>(this TryAsync<T> fn)
        {
            if (fn == null) { throw new ArgumentNullException(nameof(fn)); }

            try
            {
                return await fn().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                return new Result<T>(exceptionDispatchInfo);
            }
        }

        public static Result<R> TryRun<R>(this Func<R> fn)
        {
            if (fn == null) { throw new ArgumentNullException(nameof(fn)); }

            Try<R> doCall = () => fn();
            return doCall.Try();
        }

        public static async Task<Result<R>> TryRunAsync<R>(this Func<Task<R>> fn)
        {
            if (fn == null) { throw new ArgumentNullException(nameof(fn)); }

            TryAsync<R> doCall = async () => await fn().ConfigureAwait(false);
            return await doCall.TryAsync().ConfigureAwait(false);
        }

        public static Some<R> Map<T, R>(this in Some<T> some, Func<T, R> map)
        {
            if (map == null) { throw new ArgumentNullException(nameof(map)); }

            return map(some.Value);
        }
    }
}
