using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PBilek.ORM.Core.Entity;
using PBilek.ORM.EntityFrameworkCore.Context;
using PBilek.ORM.EntityFrameworkCore.Entity;

namespace Headlines.ORM.Core.Context
{
    public sealed class HeadlinesDbContext : EfCoreDbContext
    {
        public HeadlinesDbContext(DbContextOptions<HeadlinesDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascaddeForeignKeys = modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (IMutableForeignKey mutableFk in cascaddeForeignKeys)
            {
                mutableFk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntity<>)))
                {
                    IMutableProperty idProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IEntity<long>.Id));
                    idProperty.SetColumnName("ID");
                }

                if (entityType.ClrType.GetInterfaces().Any(x => x == typeof(IUsersEntity)))
                {
                    IMutableProperty userIdProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IUsersEntity.UserId));
                    userIdProperty.SetColumnName("USER_ID");
                }

                if (entityType.ClrType.GetInterfaces().Any(x => x == typeof(ISoftDeleteEntity)))
                {
                    IMutableProperty isDeletedProperty = entityType.FindProperty(nameof(ISoftDeleteEntity.IsDeleted));
                    isDeletedProperty.SetColumnName("IS_DELETED");
                    entityType.AddIndex(isDeletedProperty);

                    IMutableProperty deletedProperty = entityType.FindProperty(nameof(ISoftDeleteEntity.Deleted));
                    deletedProperty.SetColumnName("DELETED");

                    if (entityType.ClrType.GetInterfaces().Any(x => x == typeof(IUsersEntity)))
                    {
                        IMutableProperty userIdProperty = entityType.FindProperty(nameof(IUsersEntity.UserId));
                        entityType.AddIndex(new[] { userIdProperty, isDeletedProperty });
                    }
                }

                if (entityType.ClrType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISoftDeleteEntity<>)))
                {
                    IMutableProperty deleteIssuerId = entityType.GetProperties().First(prop => prop.Name == nameof(ISoftDeleteEntity<long>.DeleteIssuerId));
                    deleteIssuerId.SetColumnName("DELETE_ISSUER_ID");
                }

                if (entityType.ClrType.GetInterfaces().Any(x => x == typeof(IRecordEntity)))
                {
                    IMutableProperty createdProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IRecordEntity.Created));
                    createdProperty.SetColumnName("CREATED");

                    IMutableProperty changedProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IRecordEntity.Changed));
                    changedProperty.SetColumnName("CHANGED");
                }

                if (entityType.ClrType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRecordEntity<>)))
                {
                    IMutableProperty createIssuerIdProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IRecordEntity<long>.CreateIssuerId));
                    createIssuerIdProperty.SetColumnName("CREATE_ISSUER_ID");

                    IMutableProperty changeIssuerIdProperty = entityType.GetProperties().First(prop => prop.Name == nameof(IRecordEntity<long>.ChangeIssuerId));
                    changeIssuerIdProperty.SetColumnName("CHANGE_ISSUER_ID");
                }
            }
        }
    }
}