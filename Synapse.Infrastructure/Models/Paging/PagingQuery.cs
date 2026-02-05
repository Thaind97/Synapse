using System.ComponentModel.DataAnnotations;

namespace Synapse.Infrastructure.Models.Paging
{
    public class PagingQuery
    {
        [Range(1, 100, ErrorMessage = "Page size is positive number only")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page Index is positive number only")]
        public int PageIndex { get; set; } = 1;

        public string OrderBy { get; set; } = string.Empty;
        public bool OrderByDesc { get; set; } = false;
    }
}
