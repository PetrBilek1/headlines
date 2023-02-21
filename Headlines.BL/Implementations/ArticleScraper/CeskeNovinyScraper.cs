using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class CeskeNovinyScraper : ArticleScraperBase
    {
        public CeskeNovinyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1[contains(concat(' ', @itemprop, ' '), ' name ')]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//p[contains(concat(' ', @class, ' '), ' author ')]//a[text()]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//div[contains(concat(' ', @itemprop, ' '), ' articleBody ')]")
                .SelectNodes(".//*[(self::p or self::h3) and not(contains(concat(' ', @class, ' '), ' tags ')) and not(ancestor::div[contains(@class, 'footer') or contains(concat(' ', @class, ' '), ' infobox ')])]")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes(".//p[contains(concat(' ', @class, ' '), ' tags ')]//a[text()]")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}