using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class NovinkyScraper : ArticleScraperBase
    {
        public override ArticleScraperType ScraperType { get; } = ArticleScraperType.Novinky;

        public NovinkyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("data-dot", "ogm-article-author")}]//a[text()]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//*[{ContainsExact("data-dot", "ogm-article-perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("data-dot", "ogm-article-content")}]")
                ?.SelectNodes($".//*[{ContainsExact("data-dot", "mol-paragraph")}]")
                ?.SelectInnerText()
                .ToList() 
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("data-dot", "ogm-related-tags")}]//a[text()]")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}