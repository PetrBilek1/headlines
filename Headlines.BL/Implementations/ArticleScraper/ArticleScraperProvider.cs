using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.Enums;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class ArticleScraperProvider : IArticleScraperProvider
    {
        public IArticleScraper Provide(ArticleScraperType scraperType)
        {
            return scraperType switch
            {
                ArticleScraperType.Default => new DefaultArticleScraper(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}