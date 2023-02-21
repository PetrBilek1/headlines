using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class SeznamZpravyScraper : ArticleScraperBase
    {
        public SeznamZpravyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1")
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//div[contains(concat(' ', @data-dot, ' '), ' ogm-article-author ') or contains(concat(' ', @data-dot, ' '), ' ogm-author-box ')]")
                .SelectSingleNode("//*[contains(concat(' ', @data-dot, ' '), ' ogm-author-box__name ') or contains(concat(' ', @data-dot, ' '), ' mol-author-names ')]")
                .InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//p[contains(concat(' ', @data-dot, ' '), ' ogm-article-perex ')]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//div[contains(concat(' ', @data-dot, ' '), ' ogm-article-content ')]")
                .SelectNodes(".//div[contains(concat(' ', @data-dot, ' '), ' mol-paragraph ')]//p")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes(".//div[contains(concat(' ', @data-dot, ' '), ' ogm-related-tags ')]//div")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}