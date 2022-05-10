using System.Diagnostics.CodeAnalysis;

namespace BddPipe.Model
{
    /// <summary>
    /// Represents the state of a Pipe{T} instance when no error has occurred.
    /// </summary>
    public sealed class PipeData<T> : IScenarioResult
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

        internal PipeData(T value, in Some<ScenarioResult> result)
        {
            Value = value;
            Result = result.Value;
        }
    }
}
