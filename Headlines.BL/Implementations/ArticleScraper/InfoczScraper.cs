using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class InfoczScraper : ArticleScraperBase
    {
        public new ArticleScraperType ScraperType { get; } = ArticleScraperType.Infocz;

        public InfoczScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//footer[{ContainsExact("class", "i-paywall-overlay")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1[contains(@class, 'articles-header')]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h2[contains(@class, 'articles-author')]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//p[contains(@class, 'articles-content__perex')]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes("//div[contains(@class, 'rich-text')]/p")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes("//section[contains(@class, 'articles-content__top')]//a[contains(@class, 'layout-badge')]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
    }
}