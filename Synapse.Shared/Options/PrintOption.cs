namespace Synapse.Shared.Options
{
    public class PrintOption
    {
        public string Url { get; set; } = null!;
        public int Port { get; set; }
        public string ApiKey { get; set; } = default!;
    }
}