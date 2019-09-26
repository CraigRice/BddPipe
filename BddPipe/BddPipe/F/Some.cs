using System;

namespace BddPipe
{
    public class NotInitialisedException : Exception
    {
        public NotInitialisedException(string message) : base(message)
        {
        }
    }

    internal struct Some<T>
    {
        private readonly T _value;
        private readonly bool _initialised;

        public Some(T value)
        {
            if (value == null)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value can not be null");
            }

            _initialised = true;
            _value = value;
        }

        private static T raise<T>(Exception ex)
        {
            throw ex;
        }

        public T Value =>
            _initialised
                ? _value
                : raise<T>(new NotInitialisedException($"Some {typeof(T).Name} has not been initialised"));

        public static bool operator ==(Some<T> a, Some<T> b) => a.Value.Equals(b.Value);
        public static bool operator !=(Some<T> a, Some<T> b) => a.Value.Equals(b.Value) == false;
        public static implicit operator Some<T>(T value) => new Some<T>(value);
        public static implicit operator T(Some<T> value) => value.Value;

        public Type GetUnderlyingType() =>
            typeof(T);

        public override string ToString() =>
            Value.ToString();

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Some<T> some)
            {
                return Value.Equals(some.Value);
            }

            return Value.Equals(obj);
        }
    }
}
