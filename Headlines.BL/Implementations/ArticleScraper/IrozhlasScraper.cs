using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
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
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//p[{ContainsExact("class", "meta")}]/strong")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "b-detail")}]")
                ?.SelectNodes($".//*[(self::p or self::div or self::h2) and not(contains(@class, 'meta')) and not (ancestor::a or ancestor::figure or ancestor::div[{ContainsExact("class", "embed")}] or ancestor::div[{ContainsExact("class", "b-tweet")}] or ancestor-or-self::div[{ContainsExact("class", "b-inline")}])]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document) => new List<string>();
    }
}