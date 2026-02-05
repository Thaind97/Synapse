using System.IO;
using System.Text.Json;

namespace Synapse.Services.Services.Implement
{
    /// <summary>
    /// Token storage service for persisting authentication tokens
    /// </summary>
    public interface ITokenStorage
    {
        Task SaveTokensAsync(string accessToken, string refreshToken);
        Task<(string? AccessToken, string? RefreshToken)> LoadTokensAsync();
        Task ClearTokensAsync();
    }

    /// <summary>
    /// File-based token storage (for development/desktop apps)
    /// For production, consider using Windows Credential Manager or encrypted storage
    /// </summary>
    public class FileTokenStorage : ITokenStorage
    {
        private readonly string _tokenFilePath;

        public FileTokenStorage(string? customPath = null)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "Synapse");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _tokenFilePath = customPath ?? Path.Combine(appFolder, "tokens.json");
        }

        public async Task SaveTokensAsync(string accessToken, string refreshToken)
        {
            var tokens = new TokenData
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                SavedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(tokens);
            await File.WriteAllTextAsync(_tokenFilePath, json);
        }

        public async Task<(string? AccessToken, string? RefreshToken)> LoadTokensAsync()
        {
            try
            {
                if (!File.Exists(_tokenFilePath))
                {
                    return (null, null);
                }

                var json = await File.ReadAllTextAsync(_tokenFilePath);
                var tokens = JsonSerializer.Deserialize<TokenData>(json);

                return (tokens?.AccessToken, tokens?.RefreshToken);
            }
            catch
            {
                return (null, null);
            }
        }

        public Task ClearTokensAsync()
        {
            if (File.Exists(_tokenFilePath))
            {
                File.Delete(_tokenFilePath);
            }
            return Task.CompletedTask;
        }

        private class TokenData
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime SavedAt { get; set; }
        }
    }
}
