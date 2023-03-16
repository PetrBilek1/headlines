using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class E15Scraper : ArticleScraperBase
    {
        public E15Scraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "content")}]/div[{ContainsExact("class", "subscription--article")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("itemprop", "headline")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "author-name")}]/h3")
                .SelectNotNullOrWhiteSpaceInnerText()
                .JoinStrings()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "content")}]/p")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "keywords")}]/*[{ContainsExact("class", "keyword")}]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
    }
}