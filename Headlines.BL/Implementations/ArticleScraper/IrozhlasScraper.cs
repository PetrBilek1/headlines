using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IrozhlasScraper : ArticleScraperBase
    {
        public IrozhlasScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1[contains(@id, 'article-news-full')]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//p[{ContainsExact("class", "meta")}]/strong")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "b-detail")}]")
                .SelectNodes($".//*[(self::p or self::div or self::h2) and not(contains(@class, 'meta')) and not (ancestor::a or ancestor::figure or ancestor::div[{ContainsExact("class", "embed")}] or ancestor::div[{ContainsExact("class", "b-tweet")}] or ancestor-or-self::div[{ContainsExact("class", "b-inline")}])]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document) => new List<string>();
    }
}