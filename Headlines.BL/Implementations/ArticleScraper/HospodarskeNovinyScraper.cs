using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HospodarskeNovinyScraper : IArticleScraper
    {
        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await (new HtmlWeb().LoadFromWebAsync(url));
                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]/a[text()]");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' article-body-part ') and contains(concat(' ', @class, ' '), ' free-part ')]");
                var isPaywalled = IsPaywalled(contentNode);
                var content = GetContent(contentNode);
                var tags = GetTags(contentNode);

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
            return node.SelectSingleNode("//h1")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return node?.InnerText.Trim() ?? string.Empty;
        }

        private bool IsPaywalled(HtmlNode node)
        {
            return node.SelectNodes(".//div[contains(concat(' ', @class, ' '), ' paywall ')]")?.Any() == true;
        }

        private string GetContent(HtmlNode node)
        {
            return string.Join('\n', node.SelectNodes(".//p")?.Select(x => x.InnerText) ?? new List<string>());
        }

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//li[contains(@id, 'tag')]")?
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }               
    }
}