using Headlines.BL.Facades;
using Headlines.RSSProcessingMicroService.Services;
using NetCore.AutoRegisterDi;
using PBilek.RSSReaderService;
using System.Reflection;

namespace Headlines.RSSProcessingMicroService.DependencyResolution
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

            services.AddTransient<IRSSReaderService, RSSReaderService>();

            services.AddScoped<IRSSSourceReaderService, RSSSourceReaderService>();
            services.AddScoped<IRSSProcessorService, RSSProcessorService>();

            services.AddHostedService<ServiceWorker>();

            return services;
        }
    }
}