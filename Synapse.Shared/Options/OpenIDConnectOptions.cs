namespace Synapse.Shared.Options
{
    public class OpenIDConnectOptions
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Issuer { get; set; }
        public string? Scope { get; set; }
    }
}
