using System.Text.Json.Serialization;

namespace Synapse.Shared.Models
{
    public class DeviceTime
    {
        [JsonPropertyName("timeStart")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("trackingTime")]
        public long TrackingTime { get; set; }

        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}
