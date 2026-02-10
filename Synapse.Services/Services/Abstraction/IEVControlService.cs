using Synapse.Services.Models;

namespace Synapse.Services
{
    /// <summary>
    /// Service interface for EV Control battery operations
    /// </summary>
    public interface IEVControlService
    {
        /// <summary>
        /// Gets real-time battery data for all batteries
        /// </summary>
        Task<List<BatteryData>> GetBatteryDataAsync();

        /// <summary>
        /// Starts the EV control experiment
        /// </summary>
        Task<bool> StartExperimentAsync(string patternName);

        /// <summary>
        /// Stops the EV control experiment
        /// </summary>
        Task<bool> StopExperimentAsync();

        /// <summary>
        /// Pauses/Resumes the EV control experiment
        /// </summary>
        Task<bool> TogglePauseAsync();
    }
}
