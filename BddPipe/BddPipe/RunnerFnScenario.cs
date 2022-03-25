using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        /// <summary>
        /// This optional call starts the runner with a scenario description
        /// </summary>
        /// <param name="title">The scenario title if supplied</param>
        /// <param name="methodName">The caller method name</param>
        /// <returns>The title is lifted to the scenario instance for a following call to Given()</returns>
        public static Pipe<Scenario> Scenario([AllowNull] string title = null, [AllowNull][CallerMemberName] string methodName = null) =>
            CreatePipe(new Scenario(title ?? methodName));
    }
}
