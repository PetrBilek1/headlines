using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IdnesScraper : ArticleScraperBase
    {
        public new ArticleScraperType ScraperType { get; } = ArticleScraperType.Idnes;

        public IdnesScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode.SelectSingleNode($".//div[{ContainsExact("id", "paywall")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("itemprop", "headline")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "authors")}]")
                ?.SelectNodes($".//span[{ContainsExact("itemprop", "author")}]")
                ?.SelectInnerText()
                .JoinStrings()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "opener")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("itemprop", "articleBody")}]//*[self::p or self::h3]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("class", "art-tags")}]//a[text()]")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}