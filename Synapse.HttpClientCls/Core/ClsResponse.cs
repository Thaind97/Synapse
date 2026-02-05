using System.Net;

namespace Synapse.HttpClientCls.Core
{
    public class ClsResponse
    {
        public IEnumerable<ClsHeader> Headers { get; set; }
        public CookieCollection Cookies { get; set; }
        public ClsBody ResponseBody { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public byte[]? ByteArray { get; set; }
        public ClsResponse()
        {
        }
        public ClsResponse(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
