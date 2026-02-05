using Synapse.Models.Accounts;
using Synapse.Services.Models;

namespace Synapse.Services.Services.Abstraction
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Login with username and password
        /// </summary>
        Task<AuthResult> LoginAsync(string username, string password);

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        Task<AuthResult> RefreshTokenAsync();

        /// <summary>
        /// Logout and clear tokens
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get current access token
        /// </summary>
        string? GetAccessToken();

        /// <summary>
        /// Get current refresh token
        /// </summary>
        string? GetRefreshToken();

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Current logged in user info
        /// </summary>
        TokenModel? CurrentUser { get; }
    }
}
