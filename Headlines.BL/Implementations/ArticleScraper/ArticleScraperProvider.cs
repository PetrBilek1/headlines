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
                ArticleScraperType.Blesk => new BleskScraper(_htmlDocumentLoader),
                ArticleScraperType.Denikcz => new DenikczScraper(_htmlDocumentLoader),
                ArticleScraperType.SeznamZpravy => new SeznamZpravyScraper(_htmlDocumentLoader),
                ArticleScraperType.Lidovky => new LidovkyScraper(_htmlDocumentLoader),
                ArticleScraperType.DenikReferendum => new DenikReferendumScraper(_htmlDocumentLoader),
                ArticleScraperType.A2larm => new A2larmScraper(_htmlDocumentLoader),
                ArticleScraperType.DenikN => new DeniknScraper(_htmlDocumentLoader),
                ArticleScraperType.E15 => new E15Scraper(_htmlDocumentLoader),
                ArticleScraperType.ParlamentniListy => new ParlamentniListyScraper(_htmlDocumentLoader),
                ArticleScraperType.Infocz => new InfoczScraper(_htmlDocumentLoader),
                ArticleScraperType.Hlidacipes => new HlidacipesScraper(_htmlDocumentLoader),
                _ => throw new NotImplementedException(),
            };
        }
    }
}