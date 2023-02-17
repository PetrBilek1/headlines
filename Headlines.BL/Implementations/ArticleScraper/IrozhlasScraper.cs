using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IrozhlasScraper : ArticleScraperBase
    {
        public IrozhlasScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//p[contains(concat(' ', @class, ' '), ' meta ')]/strong");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' b-detail ')]");
                var paragraphs = GetParagraphs(contentNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = false,
                    Title = title,
                    Author = author,
                    Paragraphs = paragraphs,
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

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//*[(self::p or self::div) and not(contains(@class, 'meta')) and not (ancestor::a or ancestor::figure or ancestor::div[contains(concat(' ', @class, ' '), ' embed ')] or ancestor::div[contains(concat(' ', @class, ' '), ' b-tweet ')])]")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList() 
                ?? new List<string>();
        }
    }
}