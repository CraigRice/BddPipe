namespace BddPipe
{
    /// <summary>
    /// Step outcome
    /// </summary>
    public enum Outcome
    {
        /// <summary>
        /// Step passed successfully
        /// </summary>
        Pass,
        /// <summary>
        /// Step failed when executed
        /// </summary>
        Fail,
        /// <summary>
        /// Step was inconclusive when executed
        /// </summary>
        Inconclusive,
        /// <summary>
        /// Step was not run due to a failed or inconclusive previous step
        /// </summary>
        NotRun
    }
}
