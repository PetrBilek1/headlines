using Microsoft.Extensions.DependencyInjection;
using PBilek.Infrastructure.DatetimeProvider;

namespace Headlines.DependencyResolution
{
    public static class BasicServiceCollection
    {
        public static IServiceCollection AddBasicDependencyGroup(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DefaultDateTimeProvider>();

            return services;
        }
    }
}