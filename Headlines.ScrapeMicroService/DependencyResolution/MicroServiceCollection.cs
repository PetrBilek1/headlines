using Headlines.BL.Facades;
using NetCore.AutoRegisterDi;
using PBilek.Infrastructure.DatetimeProvider;
using System.Reflection;

namespace Headlines.ScrapeMicroService.DependencyResolution
{
    public static class MicroServiceCollection
    {
        public static IServiceCollection AddMicroServiceDependencyGroup(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DefaultDateTimeProvider>();

            Assembly?[] assembliesToScan = new[]
            {
                Assembly.GetAssembly(typeof(IArticleSourceFacade)),
            };

            services
                .RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(x => x.Name.EndsWith("Facade") || x.Name.EndsWith("DAO"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

            services.AddHostedService<ServiceWorker>();

            return services;
        }
    }
}