using System;

namespace BddPipe
{
    internal static class StringExtensions
    {
        private static bool StartsWithIgnoreCase(this Some<string> text, Some<string> prefix) =>
            text.Value.IndexOf(prefix.Value, StringComparison.InvariantCultureIgnoreCase) == 0;

        public static Some<string> WithPrefix(this Option<string> text, Some<string> prefix) =>
            text.Match(txt =>
            {
                Some<string> someText = txt;

                var result = someText.StartsWithIgnoreCase(prefix)
                    ? someText
                    : new Some<string>($"{prefix} {txt}".TrimEnd());

                return result;
            }, () => prefix);
    }
}
