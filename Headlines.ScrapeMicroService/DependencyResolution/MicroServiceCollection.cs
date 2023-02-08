using Headlines.BL.Facades;
using NetCore.AutoRegisterDi;
using System.Reflection;

namespace Headlines.ScrapeMicroService.DependencyResolution
{
    public static class MicroServiceCollection
    {
        public static IServiceCollection AddMicroServiceDependencyGroup(this IServiceCollection services)
        {
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