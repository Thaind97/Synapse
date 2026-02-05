using CommunityToolkit.Mvvm.Messaging;
using Flurl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Synapse.Infrastructure.Repository.Abstraction;
using Synapse.Models.Accounts;
using Synapse.Shared.Extensions;
using Synapse.Shared.Helper;
using Synapse.Shared.Options;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Synapse.HttpClientCls.Core
{
    public class ClsRequest
    {
        public HeaderCollection Headers { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public ParameterCollection QueryParams { get; private set; }
        public ParameterCollection PostParams { get; private set; }
        public ByteArrayContent ByteArrayContent { get; private set; }
        public Uri Uri { get; private set; }
        public string JsonBody { get; private set; }
        public MultipartFormDataContent FormDataContent { get; private set; }
        public TimeSpan TimeOut { get; private set; }
        public IdentityOptions IdentityOptions { get; set; }
        public ClsRequest(Uri uri) : this(uri, null)
        {
        }

        public ClsRequest(Uri baseUri, string path)
        {
            Init();
            InitUri(baseUri, path);
        }

        private void Init()
        {
            Headers = new HeaderCollection();
            QueryParams = new ParameterCollection();
            PostParams = new ParameterCollection();
            Cookies = new CookieCollection();
        }

        private void InitUri(Uri baseUri, string path)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException("baseUri needs to be specified");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Uri = baseUri;
                return;
            }

            if (baseUri.AbsoluteUri.Contains('?'))
            {
                throw new ArgumentException("Cannot append a path when the base URI contains query parameters.");
            }

            Uri = new Uri(baseUri, path);
        }

        public virtual ClsRequest AppendPathSegment(string path)
        {
            var schemeHostAndPortPart = Uri.GetLeftPart(UriPartial.Authority);
            var url = Url.Combine(schemeHostAndPortPart, Uri.AbsolutePath, path);
            Uri = new Uri(url);
            return this;
        }

        public virtual ClsRequest AddingHeader(string key, string value)
        {
            Headers.Add(new ClsHeader(key, value));
            return this;
        }

        public virtual ClsRequest AddingHeaders(object headers)
        {
            Headers.AddRange(headers.ToClsHeaders());
            return this;
        }

        public virtual ClsRequest AddingAuthentication(string jwtToken)
        {
            Headers.Add(new ClsHeader("Authorization", "Bearer " + jwtToken));
            return this;
        }

        public virtual ClsRequest AddingHeaders(HeaderCollection headers)
        {
            Headers.AddRange(headers);
            return this;
        }

        public ClsRequest SetHeaders(HeaderCollection headers)
        {
            Headers = new HeaderCollection(headers);
            return this;
        }

        public virtual ClsRequest RemovingHeader(string key)
        {
            var toRemove = Headers.Where(h => h.Key == key).ToList();
            Headers = new HeaderCollection(Headers.Except(toRemove));
            return this;
        }

        public virtual ClsRequest SetTimeout(TimeSpan timeout)
        {
            TimeOut = timeout;
            return this;
        }

        public virtual ClsRequest SetTimeout(int minutes = 0, int seconds = 0)
        {
            TimeOut = new TimeSpan(hours: 0, minutes: minutes, seconds: seconds);
            return this;
        }

        public ClsRequest AsAjaxRequest()
        {
            RemovingHeader("X-Requested-With");
            AddingHeader("X-Requested-With", "XMLHttpRequest");
            return this;
        }

        public ClsRequest WithJsonResponse()
        {
            AsAjaxRequest();
            RemovingHeader("Accept");
            RemovingHeader("Content-Type");
            AddingHeader("Accept", "application/json, text/plain, */*");
            AddingHeader("Content-Type", "application/json");

            return this;
        }

        public ClsRequest AsAjaxRequestWithJsonResponse()
        {
            return AsAjaxRequest().WithJsonResponse();
        }

        public virtual ClsRequest SetQueryParams(ParameterCollection values)
        {
            var nameValueCollection = values.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value as string)).ToList();
            var content = new FormUrlEncodedContent(nameValueCollection);
            var queryStr = content.ReadAsStringAsync().Result;

            // http://stackoverflow.com/questions/21640/net-get-protocol-host-and-port
            var schemeHostAndPortPart = Uri.GetLeftPart(UriPartial.Authority);

            var uri = new Uri(schemeHostAndPortPart + Uri.AbsolutePath + "?" + queryStr);

            QueryParams = values;
            Uri = uri;

            return this;
        }

        public virtual ClsRequest AddingCookie(string key, string value)
        {
            throw new NotImplementedException();
        }

        public virtual ClsRequest AddingCookies(CookieCollection cookies)
        {
            Cookies = cookies;
            return this;
        }

        public ClsRequest SetContent(ParameterCollection parameters)
        {
            PostParams = parameters;
            return this;
        }

        public virtual ClsRequest SetJsonBody(string json)
        {
            JsonBody = json;
            return this;
        }

        public virtual ClsRequest SetByteArrayContent(ByteArrayContent content)
        {
            ByteArrayContent = content;
            return this;
        }

        public virtual ClsRequest SetMultipartFormDataBody(List<MultipartFormDataProperty> props)
        {
            FormDataContent = new MultipartFormDataContent();

            foreach (var prop in props)
            {
                HttpContent content = null;
                if (prop.ContentTransmitType == MultipartFormDataProperty.ContentTransmitTypeEnum.AsString)
                {
                    content = new StringContent(prop.PropertyValue);
                    content.Headers.Add("Content-Disposition", $"form-data; name=\"{prop.PropertyName}\"");
                    FormDataContent.Add(content, prop.PropertyName);
                }
                if (prop.ContentTransmitType == MultipartFormDataProperty.ContentTransmitTypeEnum.AsFile)
                {
                    if (!File.Exists(prop.PropertyValue)) continue;
                    var pr = File.ReadAllText(prop.PropertyValue);

                    if (prop.ContentStreamType == MultipartFormDataProperty.ContentStreamTypeEnum.Stream)
                    {
                        content = new StreamContent(File.OpenRead(prop.PropertyValue));
                        content.Headers.ContentType = MediaTypeHeaderValue.Parse(prop.ContentType);

                    }
                    else if (prop.ContentStreamType == MultipartFormDataProperty.ContentStreamTypeEnum.Text)
                    {
                        content = new StringContent(File.ReadAllText(prop.PropertyValue), Encoding.UTF8, prop.ContentType);
                    }
                    content.Headers.Add($"Content-Disposition", $"form-data; name=\"{prop.PropertyName}\"; filename=\"{Path.GetFileName(prop.PropertyValue)}\"");
                    FormDataContent.Add(content, prop.PropertyName, Path.GetFileName(prop.PropertyValue));
                }
            }
            return this;
        }

        public ClsResponse Get()
        {
            return GetAsync().Result;
        }

        public virtual async Task<ClsResponse> GetAsync()
        {
            return await SendRequestAsync(HttpMethod.Get);
        }

        public virtual ClsResponse Post()
        {
            try
            {
                return PostAsync().Result;
            }
            catch (Exception ex)
            {
                if (!ex.ToString().Contains("A task was canceled"))
                {
                    throw ex;
                }
                else
                {
                    throw new Exception($"The query timed out performing the action with a message 'A task was canceled from Server'");
                }
            }
        }

        public virtual async Task<ClsResponse> PostAsync()
        {
            return await SendRequestAsync(HttpMethod.Post);
        }
        public virtual async Task<ClsResponse> PatchAsync()
        {
            return await SendRequestAsync(HttpMethod.Patch);
        }
        public virtual ClsResponse Put()
        {
            return PutAsync().Result;
        }

        public virtual async Task<ClsResponse> PutAsync(int timeOutSeconds = 30)
        {
            return await SendRequestAsync(HttpMethod.Put, timeOutSeconds: timeOutSeconds);
        }

        public virtual ClsResponse Delete()
        {
            return DeleteAsync().Result;
        }

        public virtual async Task<ClsResponse> DeleteAsync()
        {
            return await SendRequestAsync(HttpMethod.Delete);
        }

        public ClsResponse Download(string filePath, HttpMethod method)
        {
            return DownloadAsync(filePath, method).Result;
        }

        public virtual async Task<ClsResponse> DownloadAsync(string filePath, HttpMethod method, IProgress<double> progress = null)
        {
            return await SendRequestAsync(method, requestToDownloadfile: filePath, timeOutSeconds: 90 * 60, progress: progress);
        }

        public virtual async Task<ClsResponse> DownloadAsByteArrayAsync(HttpMethod method)
        {
            return await SendRequestAsync(method, isDownloadAsByteArray: true);
        }

        public virtual async Task<ClsResponse> UploadAsByteArrayAsync(HttpMethod method)
        {
            return await SendRequestAsync(method, timeOutSeconds: 90 * 60);
        }

        private async Task<ClsResponse> SendRequestAsync(HttpMethod httpMethod, bool allowAutoRedirects = true, string requestToDownloadfile = null, bool isDownloadAsByteArray = false, int timeOutSeconds = 30, IProgress<double> progress = null)
        {
            var logger = ServiceHelper.Resolve<ILogger<ClsRequest>>();
            logger.LogInformation("SendRequestAsync {0}", Uri.AbsoluteUri);
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = allowAutoRedirects,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                SslProtocols = System.Security.Authentication.SslProtocols.None
            };
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            var client = new HttpClient(httpClientHandler);

            if (TimeOut.Ticks > 0)
            {
                client.Timeout = TimeOut;
            }
            else
            {
                client.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
            }

            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }

            foreach (Cookie cookie in Cookies)
            {
                httpClientHandler.CookieContainer.Add(cookie);
            }

            HttpResponseMessage httpResponseMessage = null;

            if (httpMethod == HttpMethod.Post)
            {
                if (FormDataContent != null)
                {
                    httpResponseMessage = await client.PostAsync(Uri.AbsoluteUri, FormDataContent);
                }
                else if (ByteArrayContent != null)
                {
                    var request = new HttpRequestMessage(httpMethod, Uri);
                    request.Content = ByteArrayContent;
                    httpResponseMessage = await client.SendAsync(request);
                }
                else if (string.IsNullOrEmpty(JsonBody))
                {
                    var content = new FormUrlEncodedContent(PostParams.Select(kvp => new KeyValuePair<string, string>(kvp.Key, (string)kvp.Value)));
                    httpResponseMessage = await client.PostAsync(Uri.AbsoluteUri, content);
                }
                else
                {

                    var request = new HttpRequestMessage(httpMethod, Uri);
                    request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                    httpResponseMessage = await client.SendAsync(request);
                }
            }
            if (httpMethod == HttpMethod.Patch)
            {
                if (!string.IsNullOrEmpty(JsonBody))
                {

                    var request = new HttpRequestMessage(httpMethod, Uri);
                    request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                    httpResponseMessage = await client.SendAsync(request);
                }
            }
            else if (httpMethod == HttpMethod.Get)
            {
                if (requestToDownloadfile.IsEmpty())
                {
                    httpResponseMessage = await client.GetAsync(Uri);
                }
                else
                {
                    httpResponseMessage = await client.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead);
                }
            }
            else if (httpMethod == HttpMethod.Delete)
            {
                httpResponseMessage = await client.DeleteAsync(Uri.AbsoluteUri);
            }
            else if (httpMethod == HttpMethod.Put)
            {
                if (ByteArrayContent != null)
                {
                    var request = new HttpRequestMessage(httpMethod, Uri);
                    request.Content = ByteArrayContent;
                    httpResponseMessage = await client.SendAsync(request);
                }
                else if (string.IsNullOrEmpty(JsonBody))
                {
                    var content = new FormUrlEncodedContent(PostParams.Select(kvp => new KeyValuePair<string, string>(kvp.Key, (string)kvp.Value)));
                    httpResponseMessage = await client.PutAsync(Uri.AbsoluteUri, content);
                }
                else
                {
                    var request = new HttpRequestMessage(httpMethod, Uri);
                    request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                    httpResponseMessage = await client.SendAsync(request);
                }
            }

            var schemeHostAndPortPart = Uri.GetLeftPart(UriPartial.Authority);
            var uri = new Uri(schemeHostAndPortPart);

            if (!requestToDownloadfile.IsEmpty())
            {
                // check case turn off app when downloading file
                if (File.Exists(requestToDownloadfile))
                {
                    File.Delete(requestToDownloadfile);
                }

                var folderPath = Path.GetDirectoryName(requestToDownloadfile);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using var streamToReadFrom = await httpResponseMessage.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(requestToDownloadfile, FileMode.OpenOrCreate);

                var totalBytes = httpResponseMessage.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1 && progress != null;
                var totalBytesRead = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;
                int currentProgressPercentage = 0;
                do
                {
                    var bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        if (canReportProgress)
                        {
                            progress?.Report(1.0);
                        }
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (canReportProgress)
                    {
                        var progressPercentage = (int)(totalBytesRead * 100 / totalBytes);
                        if (currentProgressPercentage != progressPercentage)
                        {
                            currentProgressPercentage = progressPercentage;
                            progress?.Report(currentProgressPercentage / 100.0);
                        }
                    }
                }
                while (isMoreToRead);

                return new ClsResponse()
                {
                    ResponseCode = httpResponseMessage.StatusCode,
                    Cookies = httpClientHandler.CookieContainer.GetCookies(uri),
                    ResponseBody = null,
                    HttpResponseMessage = httpResponseMessage,
                };
            }
            if (isDownloadAsByteArray)
            {
                return new ClsResponse()
                {
                    ResponseCode = httpResponseMessage.StatusCode,
                    Cookies = httpClientHandler.CookieContainer.GetCookies(uri),
                    ResponseBody = null,
                    HttpResponseMessage = null,
                    ByteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync()
                };
            }
            var result = await httpResponseMessage.Content.ReadAsStringAsync();

            var response = new ClsResponse()
            {
                ResponseCode = httpResponseMessage.StatusCode,
                Cookies = httpClientHandler.CookieContainer.GetCookies(uri),
                ResponseBody = new ClsBody(result),
                HttpResponseMessage = httpResponseMessage,
            };

            //Call refresh token
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                WeakReferenceMessenger.Default.Send(new ForceLogoutPayload(false, "Token Expire"));
            }
            return response;
        }
    }
}
