using Headlines.BL.Facades;
using NetCore.AutoRegisterDi;
using PBilek.Infrastructure.DatetimeProvider;
using System.Reflection;

namespace Headlines.WebAPI.DependencyResolution
{
    public static class WebApiServiceCollection
    {
        public static IServiceCollection AddWebApiDependencyGroup(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();

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