using Synapse.Models;
using Synapse.Models.Tasking.Response;

namespace Synapse.Service.Tasking.Services.Abstraction
{
    public interface ITaskService
    {
        Task<ApiBaseResponse<List<TaskCommentResponse>>> GetCommentsByTaskIdAsync(long taskId);
    }
}