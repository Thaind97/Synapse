using Synapse.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Synapse.Services
{
    /// <summary>
    /// Service interface for EV Control battery operations
    /// </summary>
    public interface IEVControlService
    {
        /// <summary>
        /// Gets the initial EV control data including batteries, steps, and configuration
        /// </summary>
        Task<EVControlData> GetInitialDataAsync();

        /// <summary>
        /// Gets real-time battery data for all batteries
        /// </summary>
        Task<List<BatteryData>> GetBatteryDataAsync();

        /// <summary>
        /// Gets real-time data for a specific battery
        /// </summary>
        Task<BatteryData?> GetBatteryDataAsync(int batteryId);

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

        /// <summary>
        /// Gets the latest log entries
        /// </summary>
        Task<List<LogEntryData>> GetLogEntriesAsync(int count = 100);
    }
}
