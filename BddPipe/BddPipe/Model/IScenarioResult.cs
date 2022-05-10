using System.Diagnostics.CodeAnalysis;

namespace BddPipe.Model
{
    /// <summary>
    /// Describes the outcome after running one or many steps.
    /// </summary>
    public interface IScenarioResult
    {
        /// <summary>
        /// A full description of the scenario and step results
        /// </summary>
        [NotNull] ScenarioResult Result { get; }
    }
}
