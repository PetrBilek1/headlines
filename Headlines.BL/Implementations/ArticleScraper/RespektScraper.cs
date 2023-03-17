using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class RespektScraper : ArticleScraperBase
    {
        public new ArticleScraperType ScraperType { get; } = ArticleScraperType.Respekt;

        public RespektScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("id", "formular-predplatne")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//header//h1")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//div[contains(@class, 'AuthorLocket_heading')]")
                ?.SelectNodes("./*[text()]")
                ?.SelectInnerText()
                .JoinStrings()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[contains(@class, 'ArticleHeader_perex')]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
        {
            var nodes = document.DocumentNode
                .SelectNodes($"//div[contains(@class, 'Container_root')]//div[contains(@class, 'RichText_root')]//*[self::p or self::h2 or self::hr]")
                ?.ToList()
            ?? new List<HtmlNode>();

            var endIndex = nodes.FindLastIndex(x => x.Name == "hr");

            return endIndex > 0
                ? nodes
                    .GetRange(0, endIndex)
                    .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    .SelectInnerText()
                    .ToList()
                : nodes
                    .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    .SelectInnerText()
                    .ToList();
        }


        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//*[contains(@class, 'Tag_root')]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
    }
}