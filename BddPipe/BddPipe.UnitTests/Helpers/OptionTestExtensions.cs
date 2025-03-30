using static BddPipe.F;

namespace BddPipe.UnitTests.Helpers;

internal static class OptionTestExtensions
{
    public static Option<string> AsOption(this string? value) =>
        value == null
            ? None
            : Some(value);
}
