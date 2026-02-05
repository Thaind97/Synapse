namespace Synapse.Models.Tasking.Response
{
    public class TaskCommentResponse
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
