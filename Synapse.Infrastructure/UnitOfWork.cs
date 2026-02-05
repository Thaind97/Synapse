using Microsoft.EntityFrameworkCore.Storage;
using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Implemment;
using System.Threading.Tasks;

namespace Synapse.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private SynapseDbContext _dbContext;
        private bool _isDisposed;
        private IDbContextTransaction _objTran;
        private Dictionary<string, object> _repositories;
        public SynapseDbContext DbContext => _dbContext;

        public UnitOfWork(SynapseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync()
        {
            _objTran = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_objTran != null)
            {
                await _objTran.CommitAsync();
                await _objTran.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_objTran != null)
            {
                await _objTran.RollbackAsync();
                await _objTran.DisposeAsync();
            }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _isDisposed = true;
        }

        public BaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEmptyEntity
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new BaseRepository<TEntity>(_dbContext);
                _repositories.Add(type, repositoryInstance);
            }
            return (BaseRepository<TEntity>)_repositories[type];
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await BeginTransactionAsync();
            try
            {
                await action();
                await CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            await BeginTransactionAsync();
            try
            {
                var result = await action();
                await CommitAsync();
                return result;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }
    }
}
