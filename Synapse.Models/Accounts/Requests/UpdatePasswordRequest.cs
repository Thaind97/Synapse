namespace Synapse.Models.Accounts.Requests
{
    public class UpdatePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}