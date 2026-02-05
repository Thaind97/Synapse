using System.ComponentModel.DataAnnotations;
using Serilog;

namespace Synapse.Models.Accounts.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Account { get; set; }

        [Required]
        public string Password { get; set; }

        public string IpAddress { get; set; } = "127.0.0.1";

        public bool RememberMe { get; set; } = true;

        public long WarehouseId { get; set; }
    }
}
