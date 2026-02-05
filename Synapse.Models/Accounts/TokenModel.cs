namespace Synapse.Models.Accounts
{
    public class TokenModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string GroupCode { get; set; }
        public string[] Roles { get; set; }
        public bool IsVerified { get; set; }
        public string? JwToken { get; set; }
        public string? refresh_token { get; set; }
    }
}
