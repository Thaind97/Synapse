namespace Synapse.Models.Accounts
{
    public class UserRequestLoginModel
    {
        public required string Email { get; set; }
        public required string SerialNumber { get; set; }
    }
}