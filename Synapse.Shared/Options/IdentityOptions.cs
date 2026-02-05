namespace Synapse.Shared.Options
{
    public class IdentityOptions
    {
        public string? Url { get; set; }
        public string? ApiGatewayUrl { get; set; }
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public Func<string, string, Task> SaveToken { get; set; }
    }
}