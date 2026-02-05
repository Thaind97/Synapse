namespace Synapse.Shared.Options
{
    /// <summary>
    /// Configuration for third-party API
    /// </summary>
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }
}
