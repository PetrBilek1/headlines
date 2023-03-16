using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class ReflexScraper : ArticleScraperBase
    {
        public ReflexScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//section[{ContainsExact("class", "subscription--article")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("class", "title")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "article-top")}]//div[{ContainsExact("class", "meta")}]/span[{ContainsExact("class", "author")}]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .JoinStrings()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "content")}]/*[self::p or self::h2]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "keywords")}]/a")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}