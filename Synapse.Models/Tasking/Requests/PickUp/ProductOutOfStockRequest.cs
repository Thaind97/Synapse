namespace Synapse.Models.Tasking.Requests.PickUp
{
    public class ProductOutOfStockRequest
    {
        public long TaskId { get; set; }
        public long LocationId { get; set; }
        public long ProductId { get; set; }
    }
}
