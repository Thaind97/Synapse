namespace Synapse.Shared.Enums
{
    /// <summary>
    /// Represents the status of a step in a process
    /// </summary>
    public enum StepStatus
    {
        /// <summary>
        /// Step is pending and not yet started
        /// </summary>
        Pending,

        /// <summary>
        /// Step is currently active/in progress
        /// </summary>
        Active,

        /// <summary>
        /// Step has been completed
        /// </summary>
        Completed
    }
}
