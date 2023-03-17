using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.Enums;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DefaultArticleScraper : IArticleScraper
    {
        public ArticleScraperType ScraperType { get; } = ArticleScraperType.Default;

        public Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            throw new NotImplementedException();
        }
    }
}