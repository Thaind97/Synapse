namespace Synapse.Models.Tasking.Requests.PickUp
{
    public class AttachHUToNewSubtaskRequest
    {
        public long TaskId { get; set; }
        public long SalesOrderId { get; set; }
        public string HandlingUnitCodeScaned { get; set; } = string.Empty;

        // clone this subtask to new subtack
        public long TaskItemId { get; set; }
    }
}
