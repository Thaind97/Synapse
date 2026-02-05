namespace Synapse.Models.Accounts
{
    public class UserProfileModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string[] Roles { get; set; }
        public string[] Departments { get; set; }
        public string[] Positions { get; set; }
    }
}