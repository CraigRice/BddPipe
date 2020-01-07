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
        private readonly Ctn<Exception> _containerOfError;
        private readonly Ctn<T> _containerOfValue;

        private bool IsRight { get; }
        private bool IsLeft => !IsRight;
        private readonly bool _isInitialised;

        internal Pipe(Ctn<Exception> containerOfError)
        {
            if (containerOfError == null) throw new ArgumentNullException(nameof(containerOfError));

            IsRight = false;
            _containerOfError = containerOfError;
            _containerOfValue = default(Ctn<T>);
            _isInitialised = true;
        }

        internal Pipe(Ctn<T> containerOfValue)
        {
            if (containerOfValue == null) throw new ArgumentNullException(nameof(containerOfValue));

            IsRight = true;
            _containerOfValue = containerOfValue;
            _containerOfError = default(Ctn<Exception>);
            _isInitialised = true;
        }

        public static implicit operator Pipe<T>(Ctn<Exception> containerOfError) => new Pipe<T>(containerOfError);
        public static implicit operator Pipe<T>(Ctn<T> containerOfValue) => new Pipe<T>(containerOfValue);

        public TResult Match<TResult>(Func<Ctn<T>, TResult> containerOfValue, Func<Ctn<Exception>, TResult> containerOfError)
        {
            if (!_isInitialised)
            {
                throw new PipeNotInitialzedException();
            }
            return IsLeft ? containerOfError(_containerOfError) : containerOfValue(_containerOfValue);
        }

        public Unit Match(Action<Ctn<T>> payload, Action<Ctn<Exception>> error)
            => Match(payload.ToFunc(), error.ToFunc());

        public override string ToString() => Match(payload => $"Container of ({payload})", error => $"Container of ({error})");
    }

    public static class PipeExtensions
    {
        public static Pipe<R> Bind<T, R>(
                this Pipe<T> pipe,
                Func<Ctn<T>, Pipe<R>> bindContainerOfValue
            ) => pipe.Match(bindContainerOfValue, containerOfError => containerOfError);

        public static Pipe<R> BiBind<T, R>(
                this Pipe<T> pipe,
                Func<Ctn<T>, Pipe<R>> bindContainerOfValue,
                Func<Ctn<Exception>, Pipe<R>> bindContainerOfError
            ) => pipe.Match(bindContainerOfValue, bindContainerOfError);
    }
}
