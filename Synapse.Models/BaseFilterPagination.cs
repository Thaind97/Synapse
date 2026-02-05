namespace Synapse.Models
{
    public class BaseFilterPagination
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SearchValue { get; set; }
        public string? QueryFilter { get; set; }
    }
}
