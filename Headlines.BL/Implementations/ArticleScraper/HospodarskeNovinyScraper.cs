using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HospodarskeNovinyScraper : ArticleScraperBase
    {
        public HospodarskeNovinyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]/a[text()]");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' article-body-part ') and contains(concat(' ', @class, ' '), ' free-part ')]");
                var isPaywalled = IsPaywalled(contentNode);
                var paragraphs = GetParagraphs(contentNode);

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' tags ')]");
                var tags = GetTags(tagsNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = isPaywalled,
                    Title = title,
                    Author = author,
                    Paragraphs = paragraphs,
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

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//p")?.Select(x => x.InnerText).ToList() ?? new List<string>();
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