using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DenikReferendumScraper : ArticleScraperBase
    {
        public override ArticleScraperType ScraperType { get; } = ArticleScraperType.DenikReferendum;

        public DenikReferendumScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "header")}]//h2")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//*[{ContainsExact("class", "articleDetailAuthorName")}]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .JoinStrings()
            ?? string.Empty;


        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//*[{ContainsExact("class", "perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "text")}]/*[self::p or self::h3]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document) => new List<string>();        
    }
}