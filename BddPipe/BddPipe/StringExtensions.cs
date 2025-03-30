using System;
using static BddPipe.F;

namespace BddPipe
{
    internal static class StringExtensions
    {
        private static bool StartsWithIgnoreCase(this in Some<string> text, in Some<string> prefix) =>
            text.Value.IndexOf(prefix.Value, StringComparison.InvariantCultureIgnoreCase) == 0;

        public static Some<string> WithPrefix(this in Option<string> text, Some<string> prefix) =>
            text.Match(
                txt =>
                {
                    Some<string> someText = txt;

                    var result = someText.StartsWithIgnoreCase(prefix)
                        ? someText
                        : new Some<string>($"{prefix} {txt}".TrimEnd());

                    return result;
                },
                () => prefix
            );

        public static Option<string> NoneIfWhiteSpace(this in Option<string> text) =>
            text.Match<Option<string>>(
                txt =>
                {
                    if (string.IsNullOrWhiteSpace(txt))
                    {
                        return None;
                    }

                    return txt;
                },
                () => None
            );
    }
}
