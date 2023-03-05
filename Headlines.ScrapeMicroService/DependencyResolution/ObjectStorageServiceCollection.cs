using PBilek.ObjectStorageService.Contabo;
using PBilek.ObjectStorageService;
using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.BL.Implementations.ObjectStorageWrapper;

namespace Headlines.ScrapeMicroService.DependencyResolution
{
    public static class ObjectStorageServiceCollection
    {
        public static IServiceCollection AddObjectStorageDependencyGroup(this IServiceCollection services, ObjectStorageConfiguration objectStorageConfiguration)
        {
            services.AddTransient<IObjectStorageService, ContaboObjectStorageService>(_ =>
            {
                return new ContaboObjectStorageService(objectStorageConfiguration);
            });
            services.AddTransient<IObjectStorageWrapper, ObjectStorageWrapper>();

            return services;
        }
    }
}