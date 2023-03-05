using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HlidacipesScraper : ArticleScraperBase
    {
        public HlidacipesScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("class", "post-heading")}]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => string.Join(
                ", ",
                document.DocumentNode
                    .SelectNodes($"//div[{ContainsExact("class", "post-info")}]/a")
                    ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    ?.Select(x => x.InnerText.Trim())
                    .ToList()
                ?? new List<string>()
            );

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "post-content")}]/*[self::p or self::h3 or self::blockquote]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document) => new List<string>();        
    }
}