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
        private readonly Ctn<Exception> _error;
        private readonly Ctn<T> _value;

        private bool IsRight { get; }
        private bool IsLeft => !IsRight;
        private readonly bool _isInitialised;

        internal Pipe(Ctn<Exception> error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            IsRight = false;
            _error = error;
            _value = default(Ctn<T>);
            _isInitialised = true;
        }

        internal Pipe(Ctn<T> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            IsRight = true;
            _value = value;
            _error = default(Ctn<Exception>);
            _isInitialised = true;
        }

        public static implicit operator Pipe<T>(Ctn<Exception> error) => new Pipe<T>(error);
        public static implicit operator Pipe<T>(Ctn<T> right) => new Pipe<T>(right);

        public TResult Match<TResult>(Func<Ctn<T>, TResult> payload, Func<Ctn<Exception>, TResult> error)
        {
            if (!_isInitialised)
            {
                throw new EitherNotInitialzedException();
            }
            return IsLeft ? error(this._error) : payload(this._value);
        }

        public Unit Match(Action<Ctn<T>> payload, Action<Ctn<Exception>> error)
            => Match(payload.ToFunc(), error.ToFunc());

        public override string ToString() => Match(error => $"Error({error})", payload => $"Payload({payload})");
    }

    public static class PipeExtensions
    {
        public static Pipe<R> Bind<T, R>(this Pipe<T> pipe, Func<Ctn<T>, Pipe<R>> f) =>
            pipe.Match(r => f(r), l => l);

        public static Pipe<R> BiBind<T, R>(this Pipe<T> pipe, Func<Ctn<T>, Pipe<R>> bindPayload, Func<Ctn<Exception>, Pipe<R>> bindError) =>
            pipe.Match(bindPayload, bindError);
    }
}
