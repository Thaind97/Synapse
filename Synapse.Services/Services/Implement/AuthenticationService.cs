using Newtonsoft.Json;
using Synapse.HttpClientCls.Helpers;
using Synapse.Models;
using Synapse.Models.Accounts;
using Synapse.Models.Accounts.Requests;
using Synapse.Services.Services.Abstraction;
using Synapse.Shared.Options;
using Synapse.Services.Models;
using System.Net;

namespace Synapse.Services.Services.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ClsHttp _httpClient;
        private readonly IdentityOptions _identityOptions;
        private TokenModel? _currentUser;

        public AuthenticationService(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
            _httpClient = new ClsHttp(identityOptions, identityOptions.ApiGatewayUrl);
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_identityOptions.AccessToken);

        public TokenModel? CurrentUser => _currentUser;

        public string? GetAccessToken() => _identityOptions.AccessToken;

        public string? GetRefreshToken() => _identityOptions.RefreshToken;

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Account = username,
                    Password = password,
                    IpAddress = "127.0.0.1",
                    RememberMe = true
                };

                var response = await _httpClient.LoginAsync(loginRequest);

                if (response.Succeeded && response.Data != null)
                {
                    _currentUser = response.Data;

                    // Store tokens
                    _identityOptions.AccessToken = response.Data.JwToken;
                    _identityOptions.RefreshToken = response.Data.refresh_token;

                    // Persist tokens if SaveToken delegate is provided
                    if (_identityOptions.SaveToken != null)
                    {
                        await _identityOptions.SaveToken(
                            response.Data.JwToken ?? string.Empty,
                            response.Data.refresh_token ?? string.Empty
                        );
                    }

                    return AuthResult.Succeeded(response.Data);
                }

                return AuthResult.Failed(response.Message ?? "Login failed");
            }
            catch (Exception ex)
            {
                return AuthResult.Failed($"Login error: {ex.Message}");
            }
        }

        public async Task<AuthResult> RefreshTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_identityOptions.RefreshToken))
                {
                    return AuthResult.Failed("No refresh token available");
                }

                var request = _httpClient.PrepareRequest("/api/Account/refresh-token")
                    .SetJsonBody(JsonConvert.SerializeObject(new
                    {
                        refreshToken = _identityOptions.RefreshToken
                    }));

                var response = await request.PostAsync();

                if (response.ResponseCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ApiBaseResponse<TokenModel>>(
                        response.ResponseBody.StringContent);

                    if (result?.Succeeded == true && result.Data != null)
                    {
                        _currentUser = result.Data;

                        // Update tokens
                        _identityOptions.AccessToken = result.Data.JwToken;
                        _identityOptions.RefreshToken = result.Data.refresh_token;

                        // Persist tokens
                        if (_identityOptions.SaveToken != null)
                        {
                            await _identityOptions.SaveToken(
                                result.Data.JwToken ?? string.Empty,
                                result.Data.refresh_token ?? string.Empty
                            );
                        }

                        return AuthResult.Succeeded(result.Data);
                    }

                    return AuthResult.Failed(result?.Message ?? "Refresh token failed");
                }

                return AuthResult.Failed($"Refresh token failed: {response.ResponseCode}");
            }
            catch (Exception ex)
            {
                return AuthResult.Failed($"Refresh token error: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                // Call logout API if needed
                if (!string.IsNullOrEmpty(_identityOptions.AccessToken))
                {
                    var request = _httpClient.PrepareRequest("/api/Account/logout")
                        .AddingAuthentication(_identityOptions.AccessToken);

                    await request.PostAsync();
                }
            }
            catch
            {
                // Ignore logout API errors
            }
            finally
            {
                // Clear tokens
                _identityOptions.AccessToken = string.Empty;
                _identityOptions.RefreshToken = string.Empty;
                _currentUser = null;

                // Clear persisted tokens
                if (_identityOptions.SaveToken != null)
                {
                    await _identityOptions.SaveToken(string.Empty, string.Empty);
                }
            }
        }
    }
}
