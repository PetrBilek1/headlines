using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
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
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("data-dot", "ogm-article-author")} or {ContainsExact("data-dot", "ogm-author-box")}]")
                .SelectSingleNode($"//*[{ContainsExact("data-dot", "ogm-author-box__name")} or {ContainsExact("data-dot", "mol-author-names")}]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//p[{ContainsExact("data-dot", "ogm-article-perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("data-dot", "ogm-article-content")}]")
                ?.SelectNodes($".//div[{ContainsExact("data-dot", "mol-paragraph")}]//p")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("data-dot", "ogm-related-tags")}]//div")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}