using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.Enums;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class ArticleScraperProvider : IArticleScraperProvider
    {
        private readonly IHtmlDocumentLoader _htmlDocumentLoader;

        public ArticleScraperProvider(IHtmlDocumentLoader htmlDocumentLoader)
        {
            _htmlDocumentLoader = htmlDocumentLoader;
        }

        public IArticleScraper Provide(ArticleScraperType scraperType)
        {
            return scraperType switch
            {
                ArticleScraperType.Default => new DefaultArticleScraper(),
                ArticleScraperType.HospodarskeNoviny => new HospodarskeNovinyScraper(_htmlDocumentLoader),
                ArticleScraperType.Irozhlas => new IrozhlasScraper(_htmlDocumentLoader),
                ArticleScraperType.Idnes => new IdnesScraper(_htmlDocumentLoader),
                ArticleScraperType.Aktualne => new AktualneScraper(_htmlDocumentLoader),
                ArticleScraperType.Novinky => new NovinkyScraper(_htmlDocumentLoader),
                ArticleScraperType.CeskeNoviny => new CeskeNovinyScraper(_htmlDocumentLoader),
                _ => throw new NotImplementedException(),
            };
        }
    }
}