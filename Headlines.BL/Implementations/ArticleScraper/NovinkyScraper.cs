using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class NovinkyScraper : ArticleScraperBase
    {
        public NovinkyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @data-dot, ' '), ' ogm-article-author ')]");
                var author = GetAuthor(authorNode);

                HtmlNode openerNode = document.DocumentNode.SelectSingleNode(".//*[contains(concat(' ', @data-dot, ' '), ' ogm-article-perex ')]");
                var opener = openerNode?.InnerText.Trim() ?? string.Empty;

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @data-dot, ' '), ' ogm-article-content ')]");

                var paragraphs = new List<string> { opener };
                paragraphs.AddRange(GetParagraphs(contentNode));

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @data-dot, ' '), ' ogm-related-tags ')]");
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
            return node.SelectSingleNode("//h1")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return node.SelectSingleNode(".//a[text()]")?.InnerText.Trim() ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//*[contains(concat(' ', @data-dot, ' '), ' mol-paragraph ')]")?.Select(x => x.InnerText.Trim()).ToList() ?? new List<string>();
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