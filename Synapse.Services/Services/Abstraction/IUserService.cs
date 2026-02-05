using Synapse.Infrastructure.Entities;

namespace Synapse.Services.Services.Abstraction
{
    public interface IUserService
    {
        Task<UserData?> AuthenticateAsync(string username, string password);
    }
}
