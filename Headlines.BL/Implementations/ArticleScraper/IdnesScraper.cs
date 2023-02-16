using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Utils;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IdnesScraper : IArticleScraper
    {
        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = (await ScraperTools.LoadDocumentFromWebAsync(url))
                    .ReplaceNewLineTags()
                    .Sanitize();

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @itemprop, ' '), ' articleBody ')]");
                var isPaywalled = IsPaywalled(contentNode);
                var content = GetContent(contentNode);

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' art-tags ')]");
                var tags = GetTags(tagsNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = isPaywalled,
                    Title = title,
                    Author = author,
                    Content = content,
                    Tags = tags
                };
            }
            catch
            {
                return new ArticleScrapeResult { IsSuccess = false };
            }
        }

        private string GetTitle(HtmlNode node)
        {
            return node.SelectSingleNode("//h1[contains(concat(' ', @itemprop, ' '), ' headline ')]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return string.Join(", ", node.SelectNodes(".//span[contains(concat(' ', @itemprop, ' '), ' author ')]")?.Select(x => x.InnerText) ?? new List<string>());
        }

        private bool IsPaywalled(HtmlNode node)
        {
            return false;
        }

        private string GetContent(HtmlNode node)
        {
            return string.Join('\n', node.SelectNodes(".//p")?.Select(x => x.InnerText) ?? new List<string>());
        }

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//a[text()]")?
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }
    }
}