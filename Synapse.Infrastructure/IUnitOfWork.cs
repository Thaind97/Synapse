using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Implemment;
using System.Threading.Tasks;

namespace Synapse.Infrastructure
{
    public interface IUnitOfWork
    {
        SynapseDbContext DbContext { get; }
        BaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEmptyEntity;
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        void Save();
        Task SaveAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
