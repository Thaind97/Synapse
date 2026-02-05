namespace Synapse.Models.Tasking.Requests.Returning
{
    public class ConfirmTaskReturningByHURequest
    {
        public long TaskItemId { get; set; }
        public long HandlingUnitId { get; set; }
        public long SalesOrderId { get; set; }
        public long DestinationLocationId { get; set; }
        public decimal Quantity { get; set; }
        public string? LotNo { get; set; }
    }
}
