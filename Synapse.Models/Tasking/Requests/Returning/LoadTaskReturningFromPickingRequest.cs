namespace Synapse.Models.Tasking.Requests.Returning
{
    public class LoadTaskReturningFromPickingRequest
    {
        public long TaskId { get; set; }
        public long SalesOrderId { get; set; }
    }
}
