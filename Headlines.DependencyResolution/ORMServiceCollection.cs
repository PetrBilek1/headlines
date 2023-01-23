using Headlines.ORM.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PBilek.ORM.Core.Identity;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.Context;
using PBilek.ORM.EntityFrameworkCore.UnitOfWork;

namespace Headlines.DependencyResolution
{
    public static class ORMServiceCollection
    {
        public static IServiceCollection AddORMDependencyGroup(this IServiceCollection services, string defaultConnection)
        {
            services.AddTransient<HeadlinesDbContext>(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<HeadlinesDbContext>()
                    .UseSqlServer(defaultConnection, options => options
                        .EnableRetryOnFailure());

                return new HeadlinesDbContext(optionsBuilder.Options);
            });

            services.AddTransient<EfCoreDbContext>(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<HeadlinesDbContext>()
                    .UseSqlServer(defaultConnection, options => options
                        .EnableRetryOnFailure());

                return new HeadlinesDbContext(optionsBuilder.Options);
            });

            services.AddTransient<Func<EfCoreDbContext>>(c => delegate
            {
                var optionsBuilder = new DbContextOptionsBuilder<HeadlinesDbContext>()
                    .UseSqlServer(defaultConnection, options => options
                        .EnableRetryOnFailure());

                return new HeadlinesDbContext(optionsBuilder.Options);
            });

            services.AddScoped<IIdentityProvider>(c => null!);

            services.AddScoped<IUnitOfWorkProvider, EfCoreUnitOfWorkProvider>();

            return services;
        }

        public static IServiceCollection RemoveORMDependencyGroup(this IServiceCollection services)
        {
            services.RemoveAll<HeadlinesDbContext>();
            services.RemoveAll<EfCoreDbContext>();
            services.RemoveAll<Func<EfCoreDbContext>>();
            services.RemoveAll<IIdentityProvider>();
            services.RemoveAll<IUnitOfWorkProvider>();

            return services;
        }
    }
}