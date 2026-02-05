
namespace Synapse.Models
{
    public class ApiBaseResponse : BaseResponse
    {
        public List<ApiError>? Errors { get; set; }
    }

    public class ApiBaseResponse<T> : ApiBaseResponse
    {
        public T? Data { get; set; }
    }
}