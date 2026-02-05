namespace Synapse.Models.Tasking.Requests.Returning
{
    public class FinishTaskReturningRequest
    {
        public long TaskId { get; set; }
        public long HandlingUnitId { get; set; }
        public long WarehouseId { get; set; }
        public long CompanyId { get; set; }
    }
}
