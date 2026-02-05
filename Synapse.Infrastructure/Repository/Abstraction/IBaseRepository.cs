using Synapse.Infrastructure.Models.Paging;
using Synapse.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Synapse.Infrastructure.Repository.Abstraction
{
    public interface IBaseRepository<TEntity>
    {
        SynapseDbContext DataContext { get; }
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        void Update(TEntity entity);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> GetFirstOrDefaultWithPredicateAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstOrDefaultWithPredicateNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity GetFirstOrDefaultWithPredicate(Expression<Func<TEntity, bool>> predicate);
        TEntity GetFirstOrDefaultWithPredicateNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetOneAsync();
        Task<Paging<TEntity>> GetListAsync(PagingQuery pagingQuery, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> GetQueryable();
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void RemoveAllRows();
        void CopyProperties<TEntity>(TEntity source, TEntity destination);
    }
}
