namespace Synapse.Models
{
    public class BaseResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
    }
}
