namespace Synapse.Infrastructure.Entities
{
    public class UserData : BaseEntity
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
