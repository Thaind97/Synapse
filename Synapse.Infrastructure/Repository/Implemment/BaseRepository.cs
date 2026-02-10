using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Models.Paging;
using Synapse.Infrastructure.Persistence;
using Synapse.Infrastructure.Repository.Abstraction;
using System.Linq.Expressions;
using System.Reflection;

namespace Synapse.Infrastructure.Repository.Implemment
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEmptyEntity
    {
        protected SynapseDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        public SynapseDbContext DataContext => _dbContext;

        public BaseRepository(SynapseDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            return Task.CompletedTask;
        }

        public virtual void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<TEntity>().Update(entity);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            _dbContext.Set<TEntity>().UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public virtual void RemoveAllRows()
        {
            var dbSet = _dbContext.Set<TEntity>();
            dbSet.RemoveRange(dbSet);
        }

        public virtual async Task<TEntity> GetFirstOrDefaultWithPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<TEntity> GetFirstOrDefaultWithPredicateNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual TEntity GetFirstOrDefaultWithPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public virtual TEntity GetFirstOrDefaultWithPredicateNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);
        }
        

        public virtual async Task<TEntity> GetOneAsync()
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync();
        }

        public virtual async Task<Paging<TEntity>> GetListAsync(PagingQuery pagingQuery, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<TEntity>().AsQueryable();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    queryable = queryable.Include(includeProperty);
                }
            }
            queryable = queryable.Where(predicate);

            if (pagingQuery != null && !string.IsNullOrEmpty(pagingQuery.OrderBy))
            {
                var orderByProperty = GetOrderByProperty<TEntity>(pagingQuery.OrderBy);
                if (orderByProperty != null && !pagingQuery.OrderByDesc)
                {
                    queryable = queryable.OrderBy(orderByProperty);
                }
                else if (orderByProperty != null && pagingQuery.OrderByDesc)
                {
                    queryable = queryable.OrderByDescending(orderByProperty);
                }
            }

            return new Paging<TEntity>
            {
                PageSize = pagingQuery.PageSize,
                PageIndex = pagingQuery.PageIndex,
                TotalCount = await queryable.CountAsync(),
                Result = await queryable.Skip((pagingQuery.PageIndex - 1) * pagingQuery.PageSize).Take(pagingQuery.PageSize).ToListAsync()
            };
        }

        public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> items = _dbContext.Set<TEntity>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> items = _dbContext.Set<TEntity>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items.Where(predicate);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToListAsync();
        }

        private static Expression<Func<TEntity, object>> GetOrderByProperty<TEntity>(string orderByString)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var propertySelector = Expression.Property(parameter, orderByString);
            var castedSelector = Expression.Convert(propertySelector, typeof(object));
            if (castedSelector != null)
            {
                return Expression.Lambda<Func<TEntity, object>>(castedSelector, parameter);
            }

            return null;
        }

        public void CopyProperties<T>(T source, T destination)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(destination, value);
                }
            }
        }
    }
}
