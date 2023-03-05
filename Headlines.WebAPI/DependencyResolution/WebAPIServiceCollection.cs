using Headlines.BL.Facades;
using NetCore.AutoRegisterDi;
using System.Reflection;

namespace Headlines.WebAPI.DependencyResolution
{
    public static class WebAPIServiceCollection
    {
        public static IServiceCollection AddWebAPIDependencyGroup(this IServiceCollection services)
        {
            Assembly?[] assembliesToScan = new[]
            {
                Assembly.GetAssembly(typeof(IArticleSourceFacade)),
            };

            services
                .RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(x => x.Name.EndsWith("Facade") || x.Name.EndsWith("DAO"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

            return services;
        }
    }
}