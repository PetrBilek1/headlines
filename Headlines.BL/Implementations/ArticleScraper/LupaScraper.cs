using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class LupaScraper : ArticleScraperBase
    {
        public new ArticleScraperType ScraperType { get; } = ArticleScraperType.Lupa;

        public LupaScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "design-tile")}]//h1")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//a[contains(@class, 'item--author')]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//*[contains(@class, 'article__perex-content')]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "layout-article-content")}]//*[(self::p or self::h2) and not(ancestor::div[{ContainsExact("class", "rs-img-center")}])]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//ul[{ContainsExact("class", "design-list--labels")}]//li")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
    }
}