using Synapse.Shared.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Synapse.Services.Models
{
    /// <summary>
    /// Represents battery data from the EV control system
    /// </summary>
    public class BatteryData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double Temperature { get; set; }
        public int StateOfCharge { get; set; }
        public int PassCount { get; set; }
        public BatteryStatus Status { get; set; } = BatteryStatus.Normal;
        public List<double> VoltageHistory { get; set; } = new();
        public List<double> CurrentHistory { get; set; } = new();
    }

    /// <summary>
    /// Represents a step in the EV control process
    /// </summary>
    public class StepData
    {
        public string Step { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StepStatus Status { get; set; } = StepStatus.Pending;
    }

    /// <summary>
    /// Represents a log entry in the EV control system
    /// </summary>
    public class LogEntryData
    {
        public string Timestamp { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public LogLevel Level { get; set; } = LogLevel.Info;
    }

    /// <summary>
    /// Represents the complete EV control data
    /// </summary>
    public class EVControlData
    {
        public List<BatteryData> Batteries { get; set; } = new();
        public List<StepData> Steps { get; set; } = new();
        public List<LogEntryData> LogEntries { get; set; } = new();
        public string PatternName { get; set; } = string.Empty;
    }
}
