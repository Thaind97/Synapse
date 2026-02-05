namespace Synapse.Models.Tasking.Response.PickUp;

public class GetOrCreateHUForTaskRequest
{
    public long TaskId { get; set; }
    public long SalesOrderId { get; set; }
    public string HandlingUnitCodeScaned { get; set; }
}