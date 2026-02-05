namespace Synapse.Infrastructure.Models.Paging
{
    public class Paging<T>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long TotalCount { get; set; } = 0;
        public IEnumerable<T> Result { get; set; } = new List<T>();

        public Paging(int pageIndex, int pageSize, long totalCount, IEnumerable<T> result) : this(pageIndex, pageSize)
        {
            TotalCount = totalCount;
            Result = result;
        }

        public Paging(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = 0;
            Result = new List<T>();
        }

        public Paging()
        {
        }
    }
}
