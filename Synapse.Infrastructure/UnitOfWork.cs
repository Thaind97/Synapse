using Microsoft.EntityFrameworkCore.Storage;
using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Implemment;

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

        public void CreateTransaction()
        {
            _objTran = _dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (_objTran != null)
            {
                _objTran.Commit();
            }
        }

        public void Rollback()
        {
            if (_objTran != null)
            {
                _objTran.Rollback();
                _objTran.Dispose();
            }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
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
    }
}
