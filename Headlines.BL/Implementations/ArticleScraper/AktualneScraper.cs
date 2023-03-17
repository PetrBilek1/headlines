using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class AktualneScraper : ArticleScraperBase
    {
        public new ArticleScraperType ScraperType { get; } = ArticleScraperType.Aktualne;

        public AktualneScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "author")}]//*[{ContainsExact("class", "author__name")}]")
                .SelectInnerText();

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "article__perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
        {
            var contentNode = document.DocumentNode.SelectSingleNode($".//div[{ContainsExact("class", "article__content")}]");            
            contentNode ??= document.DocumentNode.SelectSingleNode($".//div[{ContainsExact("class", "article")}]/div[{ContainsExact("id", "root")}]");

            return contentNode
                .SelectNodes(".//p")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
        }

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("class", "taglist")}]//a[text()]")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}