using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Abstraction;

namespace Synapse.Infrastructure.Repository.Implemment
{
    public class UserDataRepository : BaseRepository<UserData>, IUserDataRepository
    {
        public UserDataRepository(SynapseDbContext dbContext) : base(dbContext)
        {
        }
    }
}