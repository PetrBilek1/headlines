using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class NovinkyScraper : ArticleScraperBase
    {
        public NovinkyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
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
                .SelectSingleNode($"//div[{ContainsExact("data-dot", "ogm-article-author")}]//a[text()]")
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document) 
            => document.DocumentNode
                .SelectSingleNode($".//*[{ContainsExact("data-dot", "ogm-article-perex")}]")
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("data-dot", "ogm-article-content")}]")
                ?.SelectNodes($".//*[{ContainsExact("data-dot", "mol-paragraph")}]")
                ?.Select(x => x.InnerText.Trim())
                .ToList() 
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("data-dot", "ogm-related-tags")}]//a[text()]")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}