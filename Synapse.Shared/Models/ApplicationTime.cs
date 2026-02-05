using System.Text.Json.Serialization;

namespace Synapse.Shared.Models
{
    public class ApplicationTime
    {
        [JsonPropertyName("processName")]
        public string? ProcessName { get; set; }
        [JsonPropertyName("appVersion")]
        public string? AppVersion { get; set; }

        [JsonPropertyName("trackingTime")]
        public long TrackingTime { get; set; }

        [JsonPropertyName("applicationName")]
        public string? ApplicationName { get; set; }

        [JsonPropertyName("timeUtcLog")]
        public DateTime TimeUtcLog { get; set; }
    }
}
