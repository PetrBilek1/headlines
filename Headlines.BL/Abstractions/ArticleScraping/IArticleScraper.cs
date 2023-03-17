using Headlines.Enums;

namespace Headlines.BL.Abstractions.ArticleScraping
{
    public interface IArticleScraper
    {
        ArticleScraperType ScraperType { get; }
        Task<ArticleScrapeResult> ScrapeArticleAsync(string url);
    }
}