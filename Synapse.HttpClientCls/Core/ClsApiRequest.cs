using System.Security.Cryptography;
using System.Text;

namespace Synapse.HttpClientCls.Core
{
    public class ApiEndpoint
    {
        public string? ApiKey { get; set; }
        public string? UserId { get; set; }
        public string? ApiUrl { get; set; }
    }

    public class ClsApiRequest : ClsRequest
    {
        private ApiEndpoint ApiEndpoint { get; set; }

        public ClsApiRequest(ApiEndpoint apiEndpoint) : base(new Uri(apiEndpoint.ApiUrl))
        {
            ApiEndpoint = apiEndpoint;
        }

        public override async Task<ClsResponse> GetAsync()
        {
            AddAuthorisationHeader("GET");

            return await base.GetAsync();
        }

        public override async Task<ClsResponse> PostAsync()
        {
            AddAuthorisationHeader("POST");

            return await base.PostAsync();
        }

        private void AddAuthorisationHeader(string verb)
        {
            RemovingHeader("Authorization");

            // format timestamp according to RFC1123
            var timestampStr = DateTime.UtcNow.ToString("r");

            AddingHeader("Authorization", CreateAuthorizationHeader(ApiEndpoint, verb, timestampStr));
            AddingHeader("Date", timestampStr);
        }

        private string CreateAuthorizationHeader(ApiEndpoint apiEndpoint, string httpMethod, string timestampStr)
        {
            var hashData = string.Format("{0}\n{1}\n{2}\n{3}\n", httpMethod, apiEndpoint.ApiUrl, apiEndpoint.UserId, timestampStr);
            var authHeader = string.Format("JanisonAPI {0}:{1}", apiEndpoint.UserId, GenerateHash(apiEndpoint.ApiKey, hashData));
            return authHeader;
        }

        private string GenerateHash(string keyString, string document)
        {
            var key = Convert.FromBase64String(keyString);
            string hashString;

            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(document));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString;
        }
    }
}
