using Newtonsoft.Json;
using Synapse.HttpClientCls.Helpers;
using Synapse.Models;
using Synapse.Models.Tasking.Response;
using Synapse.Services.Models;
using System.Net;

namespace Synapse.Service.Tasking.HttpClients
{
    public class TaskingHttpObject : HttpObject
    {
        private readonly TaskingClsHttp _taskingClsHttp;
        public TaskingHttpObject(TaskingClsHttp clsHttp) : base(clsHttp)
        {
            _taskingClsHttp = clsHttp;
        }

        // Todo Remove
        public async Task<ApiBaseResponse<string>> GetTaskAssignedAsync(long warehouseId)
        {
            var response = await _taskingClsHttp
                .PrepareRequest($"/api/v1/taskPickUp/terminal/auto-assign?warehouseId={warehouseId}")
                .AddingAuthentication(_taskingClsHttp.GetIdentityOptions().AccessToken)
                .GetAsync();

            if (response.ResponseCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<ApiBaseResponse<string>>(response.ResponseBody.StringContent);
                return result;
            }

            return new ApiBaseResponse<string>
            {
                Succeeded = false
            };
        }

        public async Task<ApiBaseResponse<List<TaskCommentResponse>>> GetCommentsByTaskIdAsync(long taskId)
        {
            var response = await _taskingClsHttp
                .PrepareRequest($"/api/v1/task/comments?taskId={taskId}")
                .AddingAuthentication(_taskingClsHttp.GetIdentityOptions().AccessToken)
                .GetAsync();

            if (response.ResponseCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<ApiBaseResponse<List<TaskCommentResponse>>>(response.ResponseBody.StringContent);
                return result;
            }

            return new ApiBaseResponse<List<TaskCommentResponse>>
            {
                Succeeded = false
            };
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            await Task.Delay(100); // Simulate network delay
            return new DashboardData
            {
                PowerGenerationTrend = new List<double> { 10, 15, 12, 18, 20, 25, 23, 30, 28, 35, 32, 40 },
                ChartLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                AlarmCount = 5
            };
        }
    }
}