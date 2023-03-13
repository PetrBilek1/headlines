using Headlines.BL.Facades;
using Headlines.RSSProcessingMicroService.Services;
using NetCore.AutoRegisterDi;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ObjectStorageService;
using PBilek.RSSReaderService;
using System.Reflection;

namespace Headlines.RSSProcessingMicroService.DependencyResolution
{
    public static class MicroServiceCollection
    {
        public static IServiceCollection AddMicroServiceDependencyGroup(this IServiceCollection services, ObjectStorageConfiguration objectStorageConfiguration)
        {
            services.AddTransient<IDateTimeProvider, DefaultDateTimeProvider>();

            Assembly?[] assembliesToScan = new[]
            {
                Assembly.GetAssembly(typeof(IArticleSourceFacade)),
            };

            services
                .RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(x => x.Name.EndsWith("Facade") || x.Name.EndsWith("Dao"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

            services.AddTransient<IRSSReaderService, RSSReaderService>();

            services.AddScoped<IRssSourceReaderService, RssSourceReaderService>();
            services.AddScoped<IRssProcessorService, RssProcessorService>();

            services.AddHostedService<ServiceWorker>();

            return services;
        }
    }
}