using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using System.Linq.Expressions;

namespace Synapse.Infrastructure.Persistence
{
    public class BaseDbContext<T> : DbContext where T : DbContext
    {
        protected BaseDbContext(DbContextOptions<T> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                // entity => entity.DeletedBy == null
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var deletedByProperty = Expression.Property(parameter, nameof(BaseEntity.DeletedBy));

                var nullConstant = Expression.Constant(null, typeof(string));
                var equalExpression = Expression.Equal(deletedByProperty, nullConstant);

                var lambda = Expression.Lambda(equalExpression, parameter);

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetEntitiesAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken()
        )
        {
            SetEntitiesAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected void SetEntitiesAudit()
        {
            var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = now;
                        entry.Property(x => x.CreatedDate).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        break;
                }
            }
        }

    }
}
