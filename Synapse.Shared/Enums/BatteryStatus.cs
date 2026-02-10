namespace Synapse.Shared.Enums
{
    /// <summary>
    /// Represents the status of a battery in the EV control system
    /// </summary>
    public enum BatteryStatus
    {
        /// <summary>
        /// Normal operating status - Green border
        /// </summary>
        Normal,

        /// <summary>
        /// Warning status - Red border
        /// </summary>
        Warning,

        /// <summary>
        /// Inactive/Disabled status - Gray background
        /// </summary>
        Inactive,

        /// <summary>
        /// Selected status - Blue/Cyan border
        /// </summary>
        Selected
    }
}
