namespace Synapse.Models
{
    public class ApiBaseSearchResponse<T> : ApiBaseResponse<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCurrent { get; set; }
        public int PageCount { get; set; }
        public long TotalItems { get; set; }
    }
}
