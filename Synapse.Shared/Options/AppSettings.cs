namespace Synapse.Shared.Options
{
    /// <summary>
    /// Application settings loaded from appsettings.json
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Login mode: "API" for third-party API, "SQLite" for local database
        /// </summary>
        public string LoginMode { get; set; } = "SQLite";

        /// <summary>
        /// Base URL for third-party API
        /// </summary>
        public string ApiBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// API Gateway URL
        /// </summary>
        public string ApiGatewayUrl { get; set; } = string.Empty;

        /// <summary>
        /// Check if using API login
        /// </summary>
        public bool IsApiLogin => LoginMode.Equals("API", StringComparison.OrdinalIgnoreCase);
    }
}
