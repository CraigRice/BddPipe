using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace BddPipe.Model
{
    /// <summary>
    /// Represents the state of a Pipe{T} instance when in an error state.
    /// </summary>
    public sealed class PipeErrorData : IScenarioResult
    {
        /// <summary>
        /// Pipe{T} exception data.
        /// </summary>
        [NotNull]
        public ExceptionDispatchInfo ExceptionDispatchInfo { get; }

        /// <summary>
        /// A full description of the scenario and step results
        /// </summary>
        [NotNull]
        public ScenarioResult Result { get; }

        internal PipeErrorData(
            in Some<ExceptionDispatchInfo> exceptionDispatchInfo,
            in Some<ScenarioResult> result)
        {
            ExceptionDispatchInfo = exceptionDispatchInfo.Value;
            Result = result.Value;
        }
    }
}
