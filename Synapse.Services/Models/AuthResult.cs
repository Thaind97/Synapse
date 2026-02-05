using Synapse.Models.Accounts;

namespace Synapse.Services.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TokenModel? User { get; set; }

        public static AuthResult Succeeded(TokenModel user) => new() { Success = true, User = user };
        public static AuthResult Failed(string error) => new() { Success = false, ErrorMessage = error };
    }
}
