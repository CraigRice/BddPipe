using System.Diagnostics.CodeAnalysis;

namespace BddPipe
{
    /// <summary>
    /// Represents the state of a Pipe{T} instance when no error has occurred.
    /// </summary>
    public sealed class PipeState<T> : IScenarioResult
    {
        /// <summary>
        /// Pipe{T} current value.
        /// </summary>
        [MaybeNull]
        public T Value { get; }

        /// <summary>
        /// A full description of the scenario and step results
        /// </summary>
        [NotNull]
        public ScenarioResult Result { get; }

        internal PipeState(T value, Some<ScenarioResult> result)
        {
            Value = value;
            Result = result.Value;
        }
    }
}
