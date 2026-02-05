using Synapse.HttpClientCls.Core;
using System.Net;

namespace Synapse.HttpClientCls.Helpers
{
    public abstract class HttpObject
    {
        public ClsHttp ClsHttp { get; private set; }
        protected object _lock = new object();

        public HttpObject(ClsHttp clsHttp)
        {
            ClsHttp = clsHttp;
        }

        protected bool CanAccessUrl(string url)
        {
            var status = GetAccessStatusForUrl(null, url);
            if (status == HttpStatusCode.OK)
            {
                return true;
            }
            else if (status == HttpStatusCode.Forbidden)
            {
                return false;
            }
            else
            {
                throw new Exception(string.Format("Access to the URL has some invalid. User: {0}, Url: {1}, Status: {2}", ClsHttp.Username, url, status.ToString()));
            }
        }

        protected HttpStatusCode GetAccessStatusForUrl(ParameterCollection pageParams, string url)
        {
            var pageResponse = ClsHttp
                    .PrepareRequest(url)
                    .Get();

            return pageResponse.ResponseCode;
        }
    }
}
