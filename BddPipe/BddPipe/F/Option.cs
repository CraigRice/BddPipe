using System;
using System.Collections.Generic;
using static BddPipe.F;

namespace BddPipe
{
    internal struct OptionNone
    {
        internal static readonly OptionNone Default = new OptionNone();
    }

    internal struct Option<T> : IEquatable<OptionNone>, IEquatable<Option<T>>
    {
        private readonly T value;
        private readonly bool _isSome;
        private bool isNone => !_isSome;

        private Option(T value)
        {
            if (value == null)
                throw new ArgumentNullException();
            _isSome = true;
            this.value = value;
        }

        public static implicit operator Option<T>(OptionNone _) => new Option<T>();
        public static implicit operator Option<T>(Some<T> some) => new Option<T>(some.Value);

        public static implicit operator Option<T>(T value)
        {
            if (value == null)
            {
                return None;
            }

            return new Some<T>(value);
        }

        public bool IsSome => _isSome;

        public R Match<R>(Func<T, R> some, Func<R> none)
            => _isSome ? some(value) : none();

        public IEnumerable<T> AsEnumerable()
        {
            if (_isSome) yield return value;
        }

        public bool Equals(Option<T> other)
            => _isSome == other._isSome
               && (isNone || value.Equals(other.value));

        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        public bool Equals(OptionNone other) => isNone;

        public override string ToString() => _isSome ? $"Some({value})" : "None";
    }

    internal static class OptionExt
    {
        public static Option<R> Map<T, R>
            (this Option<T> optT, Func<T, R> f)
            => optT.Match<Option<R>>(
                (t) => Some(f(t)),
                () => None);

        public static Option<R> Bind<T, R>
            (this Option<T> optT, Func<T, Option<R>> f)
            => optT.Match(
                (t) => f(t),
                () => None);

        public static T IfNone<T>(this Option<T> option, T valueIfNone) =>
            option.Match(t => t, () => valueIfNone);

        public static int CompareTo<T>(this Option<T> optT, Option<T> optCompareTo, IComparer<T> comparer)
            => optT.Match(
                t => optCompareTo.Match(
                    oct => comparer.Compare(t, oct),
                    () => 1
                ),
                () => optCompareTo.Match(
                    oct => -1,
                    () => 0
                )
            );
    }
}
