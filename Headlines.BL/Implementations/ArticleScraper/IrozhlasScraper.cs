using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Utils;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IrozhlasScraper : IArticleScraper
    {
        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = (await ScraperTools.LoadDocumentFromWebAsync(url))
                    .ReplaceNewLineTags()
                    .Sanitize();

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//p[contains(concat(' ', @class, ' '), ' meta ')]/strong");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' b-detail ')]");
                var content = GetContent(contentNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = false,
                    Title = title,
                    Author = author,
                    Content = content,
                    Tags = new List<string>()
                };
            }
            catch
            {
                return new ArticleScrapeResult { IsSuccess = false };
            }
        }

        private string GetTitle(HtmlNode node)
        {
            return node.SelectSingleNode("//h1[contains(@id, 'article-news-full')]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return node?.InnerText.Trim() ?? string.Empty;
        }

        private string GetContent(HtmlNode node)
        {
            return string.Join('\n', node.SelectNodes(".//p[not(contains(@class, 'meta')) and not (ancestor::a or ancestor::figure or ancestor::div[contains(concat(' ', @class, ' '), ' embed ')] or ancestor::div[contains(concat(' ', @class, ' '), ' b-tweet ')])]")?.Select(x => x.InnerText) ?? new List<string>());
        }
    }
}