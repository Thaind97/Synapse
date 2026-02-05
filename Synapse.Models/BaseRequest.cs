namespace Synapse.Models
{
    public class BaseRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string[] Fields { get; set; }
        public string? Filter { get; set; }
        public string? Sorts { get; set; }
        public string[] CriteriaFields { get; set; } = [];
        public string? CriteriaFilter { get; set; } = string.Empty;
        public string? CriteriaSorts { get; set; }
    }
}