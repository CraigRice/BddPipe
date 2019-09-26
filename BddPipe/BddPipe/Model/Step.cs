namespace BddPipe
{
    /// <summary>
    /// Defines the step type of a given step
    /// </summary>
    public enum Step
    {
        /// <summary>
        /// 'Given' step
        /// </summary>
        Given,
        /// <summary>
        /// 'When' step
        /// </summary>
        When,
        /// <summary>
        /// 'Then' step
        /// </summary>
        Then,
        /// <summary>
        /// 'And' step
        /// </summary>
        And,
        /// <summary>
        /// 'But' step
        /// </summary>
        But
    }
}
