namespace BddPipe
{
    /// <summary>
    /// An optional starting piece for a run
    /// <remarks>Adding a scenario will prepend the scenario description to the output</remarks>
    /// </summary>
    public sealed class Scenario
    {
        /// <summary>
        /// Title text will make part of the scenario description output
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Create a new instance of <see cref="Scenario"/>
        /// </summary>
        public Scenario(string title)
        {
            Title = title;
        }
    }
}
