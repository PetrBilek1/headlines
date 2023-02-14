namespace Headlines.BL.Abstractions.ArticleScraping
{
    public interface IArticleScraper
    {
        Task<ArticleScrapeResult> ScrapeArticleAsync(string url);
    }
}