using Newtonsoft.Json;
using System.Text.Json;

namespace Synapse.Models
{
    public class BasePayloadRequest<T>  where T : class
    {
        public T Payload { get; set; }

        public BasePayloadRequest(T payload)
        {
            Payload = payload;
        }

        public string ToSerialize()
        {
            //return JsonSerializer.Serialize(this);
            return JsonConvert.SerializeObject(this);
        }
    }
}
