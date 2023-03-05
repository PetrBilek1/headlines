using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.BL.Implementations.ObjectStorageWrapper;
using PBilek.ObjectStorageService.Contabo;
using PBilek.ObjectStorageService;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Headlines.WebAPI.DependencyResolution
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

        public static IServiceCollection RemoveObjectStorageDependencyGroup(this IServiceCollection services)
        {
            services.RemoveAll<IObjectStorageService>();
            services.RemoveAll<IObjectStorageWrapper>();

            return services;
        }
    }
}