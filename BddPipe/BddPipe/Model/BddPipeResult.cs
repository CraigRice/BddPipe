using System.Diagnostics.CodeAnalysis;

namespace BddPipe
{
    /// <summary>
    /// When the test is successful and no exceptions are thrown this will be the final result
    /// </summary>
    /// <typeparam name="T">Type of the last returned instance from a step</typeparam>
    public sealed class BddPipeResult<T> : IScenarioResult
    {
        /// <summary>
        /// Last returned item from a step
        /// </summary>
        [MaybeNull]
        public T Output { get; }

        /// <summary>
        /// A full description of the scenario and step results
        /// </summary>
        [NotNull]
        public ScenarioResult Result { get; }

        internal BddPipeResult(T output, Some<ScenarioResult> result)
        {
            Output = output;
            Result = result;
        }
    }
}
