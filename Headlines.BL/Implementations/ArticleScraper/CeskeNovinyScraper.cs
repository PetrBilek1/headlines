using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
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
                .SelectSingleNode($"//h1[{ContainsExact("itemprop", "name")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//p[{ContainsExact("class", "author")}]//a[text()]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("itemprop", "articleBody")}]")
                ?.SelectNodes($".//*[(self::p or self::h3) and not({ContainsExact("class", "tags")}) and not(ancestor::div[contains(@class, 'footer') or {ContainsExact("class", "infobox")}])]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//p[{ContainsExact("class", "tags")}]//a[text()]")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}