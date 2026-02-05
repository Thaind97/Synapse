using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Implemment;

namespace Synapse.Infrastructure
{
    public interface IUnitOfWork
    {
        SynapseDbContext DbContext { get; }
        BaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEmptyEntity;
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
