using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class A2LarmScraper : ArticleScraperBase
    {
        public A2LarmScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//*[{ContainsExact("itemprop", "headline")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                    .SelectNodes($"//*[{ContainsExact("class", "post-author-name")}]")
                    .SelectNotNullOrWhiteSpaceInnerText()
                    .JoinStrings();                   

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h2[{ContainsExact("class", "post-subtitle")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "entry-content")}]/*[self::p or self::h3 or self::blockquote]")
                ?.WhereInnerTextNotNullOrWhiteSpace()
                .SelectInnerText()
                .ReplaceLongWhiteSpaces()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "post-header-title")}]//span[{ContainsExact("class", "term-badge")}]")
                ?.WhereInnerTextNotNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}