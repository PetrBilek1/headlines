using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class CeskeNovinyScraper : ArticleScraperBase
    {
        public CeskeNovinyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//p[contains(concat(' ', @class, ' '), ' author ')]");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @itemprop, ' '), ' articleBody ')]");
                var paragraphs = GetParagraphs(contentNode);

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' tags ')]");
                var tags = GetTags(tagsNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = false,
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
            return node.SelectSingleNode("//h1[contains(concat(' ', @itemprop, ' '), ' name ')]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return node.SelectSingleNode(".//a[text()]")?.InnerText.Trim() ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//*[(self::p or self::h3) and not(contains(concat(' ', @class, ' '), ' tags ')) and not(ancestor::div[contains(@class, 'footer') or contains(concat(' ', @class, ' '), ' infobox ')])]")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList() 
                ?? new List<string>();
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