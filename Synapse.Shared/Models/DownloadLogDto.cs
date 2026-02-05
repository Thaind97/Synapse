using System.Text.Json.Serialization;

namespace Synapse.Shared.Models
{
    public class DownloadLogDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }

        [JsonPropertyName("tabUrl")]
        public string TabUrl { get; set; }

        [JsonPropertyName("currentPath")]
        public string CurrentPath { get; set; }

        [JsonPropertyName("targetPath")]
        public string TargetPath { get; set; }

        [JsonPropertyName("startTime")]
        public long StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public long EndTime { get; set; }

        [JsonPropertyName("receivedBytes")]
        public long ReceivedBytes { get; set; }
    }
}
