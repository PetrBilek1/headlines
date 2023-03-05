using AutoMapper;
using System.Reflection;

namespace Headlines.ScrapeMicroService.DependencyResolution
{
    public static class MappingServiceCollection
    {
        public static IServiceCollection AddMappingDependencyGroup(this IServiceCollection services)
        {
            var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            var assembliesTypes = assemblyNames
                .Where(x => x.Name!.Contains("Headlines"))
            .SelectMany(an => Assembly.Load(an).GetTypes())
                .Where(p => typeof(Profile).IsAssignableFrom(p) && p.IsPublic && !p.IsAbstract)
            .Distinct();

            var autoMapperProfiles = assembliesTypes
                .Select(p => (Profile)Activator.CreateInstance(p)!)
                .ToList();

            services.AddTransient<MapperConfiguration>(x => new MapperConfiguration(cfg =>
            {
                foreach (Profile profile in autoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            services.AddScoped<IMapper>(x => x.GetRequiredService<MapperConfiguration>().CreateMapper());

            return services;
        }
    }
}