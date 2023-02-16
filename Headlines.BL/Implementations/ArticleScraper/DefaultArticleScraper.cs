using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DefaultArticleScraper : IArticleScraper
    {
        public Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            throw new NotImplementedException();
        }
    }
}