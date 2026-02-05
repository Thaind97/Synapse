using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Services.Services.Abstraction;

namespace Synapse.Services.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly SynapseDbContext _dbContext;

        public UserService(SynapseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserData?> AuthenticateAsync(string username, string password)
        {
            return await _dbContext.UserData
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}
