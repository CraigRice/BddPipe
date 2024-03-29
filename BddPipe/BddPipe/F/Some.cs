﻿using System;

namespace BddPipe
{
    /// <summary>
    /// Raised when a <see cref="Some{T}"/> instance is used - after being created as default and not initialized by normal means.
    /// </summary>
    public class NotInitializedException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="NotInitializedException"/>
        /// </summary>
        /// <param name="message">A message describing the issue</param>
        internal NotInitializedException(string message) : base(message)
        {
        }
    }

    internal readonly struct Some<T>
    {
        private readonly T _value;
        private readonly bool _initialized;

        public Some(T value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value can not be null"); }

            _initialized = true;
            _value = value;
        }

        private static T Raise(Exception ex)
        {
            throw ex;
        }

        public T Value =>
            _initialized
                ? _value
                : Raise(new NotInitializedException($"Some {typeof(T).Name} has not been initialized"));

        public static bool operator ==(in Some<T> a, in Some<T> b) => a.Value.Equals(b.Value);
        public static bool operator !=(in Some<T> a, in Some<T> b) => a.Value.Equals(b.Value) == false;
        public static implicit operator Some<T>(T value) => new Some<T>(value);
        public static implicit operator T(in Some<T> value) => value.Value;

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
