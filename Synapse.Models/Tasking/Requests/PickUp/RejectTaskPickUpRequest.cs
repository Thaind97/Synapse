namespace Synapse.Models.Tasking.Requests.PickUp
{
    public class RejectTaskPickUpRequest
    {
        public long TaskId { get; set; }
        public long SalesOrderId { get; set; }
    }
}
