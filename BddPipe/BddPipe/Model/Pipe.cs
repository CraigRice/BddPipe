using System;

namespace BddPipe.Model
{
    public sealed class PipeNotInitialzedException : Exception
    {
        public PipeNotInitialzedException() : base("Pipe has not been initialized")
        {
        }
    }

    public struct Pipe<T>
    {
        private readonly Exception _exception;
        private readonly T _value;

        public bool IsRight { get; }
        public bool IsLeft => !IsRight;
        private readonly bool _isInitialised;

        internal Pipe(Exception exception)
        {
            IsRight = false;
            _exception = exception;
            _value = default(T);
            _isInitialised = true;
        }

        internal Pipe(T value)
        {
            IsRight = true;
            _value = value;
            _exception = default(Exception);
            _isInitialised = true;
        }

        public static implicit operator Pipe<T>(Exception left) => new Pipe<T>(left);
        public static implicit operator Pipe<T>(T right) => new Pipe<T>(right);

        public TResult Match<TResult>(Func<T, TResult> payload, Func<Exception, TResult> error)
        {
            if (!_isInitialised)
            {
                throw new EitherNotInitialzedException();
            }
            return IsLeft ? error(this._exception) : payload(this._value);
        }

        public Unit Match(Action<T> payload, Action<Exception> error)
            => Match(payload.ToFunc(), error.ToFunc());

        public override string ToString() => Match(error => $"Error({error})", payload => $"Payload({payload})");
    }

    public static class PipeExtensions
    {
        public static Pipe<R2> Bind<R, R2>(this Pipe<R> pipe, Func<R, Pipe<R2>> f) =>
            pipe.Match(r => f(r), l => l);

        public static Pipe<R2> BiBind<L, R, R2>(this Pipe<R> pipe, Func<R, Pipe<R2>> bindPayload, Func<Exception, Pipe<R2>> bindError) =>
            pipe.Match(bindPayload, bindError);
    }
}
