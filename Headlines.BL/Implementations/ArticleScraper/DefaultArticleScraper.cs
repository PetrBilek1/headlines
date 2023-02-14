using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DefaultArticleScraper : IArticleScraper
    {
        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlWeb web = new();
                HtmlDocument document = web.Load(url);

                string title = document.DocumentNode.SelectSingleNode("//h1").InnerText.Trim();
                string author = document.DocumentNode.SelectSingleNode("//div[@class='author']").InnerText.Trim();
                string content = string.Join("\n", document.DocumentNode.SelectNodes("//div[@class='article-body']//p")
                                            .Select(p => p.InnerText.Trim()));

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    Title = title,
                    Author = author,
                    Content = content
                };
            }
            catch
            {
                return new ArticleScrapeResult { IsSuccess = false };
            }
        }
    }
}