using Newtonsoft.Json;
using Synapse.HttpClientCls.Core;
using Synapse.Models;
using Synapse.Models.Accounts;
using Synapse.Models.Accounts.Requests;
using Synapse.Shared.Options;
using System.Net;

namespace Synapse.HttpClientCls.Helpers
{
    public class ClsHttp
    {
        protected readonly IdentityOptions _identityOptions;
        public Uri EnvironmentUri { get; private set; }
        public string Username { get; private set; }
        public ClsResponse LoginResponse { get; private set; }
        public HeaderCollection CommonHeaders { get; set; }

        public ClsHttp(IdentityOptions identityOptions, string? clsEnvironmentUrl = null)
        {
            _identityOptions = identityOptions;
            EnvironmentUri = new Uri(clsEnvironmentUrl ?? identityOptions.Url);
            CommonHeaders = new HeaderCollection {
                new ClsHeader("User-Agent", "PostmanRuntime/7.36.0"),
                new ClsHeader("Accept", "*/*"),
                new ClsHeader("Accept-Encoding", "gzip, deflate, br"),
                new ClsHeader("Connection", "keep-alive")
            };
        }

        public ClsRequest PrepareRequest(string path = "")
        {
            var request = new ClsRequest(EnvironmentUri, path);
            request.IdentityOptions = _identityOptions;
            request.AddingHeaders(CommonHeaders);

            if (LoginResponse != null)
            {
                request.AddingCookies(LoginResponse.Cookies);
            }

            return request;
        }

        public ClsRequest PrepareRequest(string baseUri, string path = "")
        {
            // Using builder to escape special characters
            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = path;
            var request = new ClsRequest(uriBuilder.Uri);
            request.IdentityOptions = _identityOptions;
            request.AddingHeaders(CommonHeaders);

            if (LoginResponse != null)
            {
                request.AddingCookies(LoginResponse.Cookies);
            }

            return request;
        }

        public async Task<ApiBaseResponse<TokenModel>> LoginAsync(LoginRequest loginRequest)
        {
            var postParam = JsonConvert.SerializeObject(loginRequest);
            var request = PrepareRequest("/api/Account/authenticate")
                        .SetJsonBody(postParam);

            var response = await request.PostAsync();
            LoginResponse = response;
            Username = loginRequest.Account;

            if (response.ResponseCode == HttpStatusCode.OK)
            {
                var tokenStringObject = response.ResponseBody.StringContent;
                var result = JsonConvert.DeserializeObject<ApiBaseResponse<TokenModel>>(tokenStringObject);
                return result;
            }
                
            return new ApiBaseResponse<TokenModel>();
        }

        public IdentityOptions GetIdentityOptions() => _identityOptions;
    }
}