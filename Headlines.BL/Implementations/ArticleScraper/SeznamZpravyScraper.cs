using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class SeznamZpravyScraper : ArticleScraperBase
    {
        public SeznamZpravyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @data-dot, ' '), ' ogm-article-author ') or contains(concat(' ', @data-dot, ' '), ' ogm-author-box ')]");
                var author = GetAuthor(authorNode);

                HtmlNode openerNode = document.DocumentNode.SelectSingleNode(".//p[contains(concat(' ', @data-dot, ' '), ' ogm-article-perex ')]");
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
            return node.SelectSingleNode("//*[contains(concat(' ', @data-dot, ' '), ' ogm-author-box__name ') or contains(concat(' ', @data-dot, ' '), ' mol-author-names ')]")
                .InnerText.Trim() ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//div[contains(concat(' ', @data-dot, ' '), ' mol-paragraph ')]//p")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//div")?
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }
    }
}