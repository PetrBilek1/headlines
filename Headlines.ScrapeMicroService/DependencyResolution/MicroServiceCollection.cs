using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Facades;
using Headlines.BL.Implementations.ArticleScraper;
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

            services.AddSingleton<IHtmlDocumentSanitizer, HtmlDocumentSanitizer>();
            services.AddSingleton<IHtmlDocumentLoader, HtmlDocumentLoader>();

            services.AddSingleton<IArticleScraperProvider, ArticleScraperProvider>();

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