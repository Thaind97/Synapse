using Synapse.Models;
using Synapse.Models.Tasking.Response;
using Synapse.Service.Tasking.HttpClients;
using Synapse.Service.Tasking.Services.Abstraction;
using Synapse.Shared.Options;

namespace Synapse.Service.Tasking.Services.Implement
{
    public class TaskService : ITaskService
    {
        private readonly TaskingHttpObject _taskingHttpObject;

        public TaskService(IdentityOptions identityOptions)
        {
            var clsHttp = new TaskingClsHttp(identityOptions);
            _taskingHttpObject = new TaskingHttpObject(clsHttp);
        }


        public async Task<ApiBaseResponse<List<TaskCommentResponse>>> GetCommentsByTaskIdAsync(long taskId)
        {
            try
            {
                return await _taskingHttpObject.GetCommentsByTaskIdAsync(taskId);
            }
            catch (Exception)
            {
                return new ApiBaseResponse<List<TaskCommentResponse>>();
            }
        }
    }
}
