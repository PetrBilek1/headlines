using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

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
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => string.Join(
                ", ",
                document.DocumentNode
                    .SelectNodes($"//*[{ContainsExact("class", "post-author-name")}]")
                    ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    .Select(x => x.InnerText.Trim())
                    .ToList()
                ?? new List<string>()
                );

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h2[{ContainsExact("class", "post-subtitle")}]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "entry-content")}]/*[self::p or self::h3 or self::blockquote]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => ScraperRegex.WhiteSpaceRegex().Replace(x.InnerText.Trim(), " "))
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "post-header-title")}]//span[{ContainsExact("class", "term-badge")}]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}