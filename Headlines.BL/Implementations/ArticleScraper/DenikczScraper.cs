using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DenikczScraper : ArticleScraperBase
    {
        public DenikczScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("class", "article-title")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
        {
            var authorNode = document.DocumentNode.SelectSingleNode($"//span[{ContainsExact("class", "article-info__author-name")}]");
            
            authorNode ??= document.DocumentNode.SelectSingleNode($"//a[{ContainsExact("class", "article-info__author")}]");

            return authorNode
                .SelectInnerText(false)
                .Replace("\n", "")
                .Trim()
                .ReplaceLongWhiteSpaces();
        }

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//p[{ContainsExact("class", "article-perex")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "article-text")}]//*[(self::p or self::h2) and text()]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "article-tags")}]//a[text()]")
                ?.Select(x => x.SelectInnerText(false).Replace(",", "").Trim())
                .ToList()
            ?? new List<string>();
    }
}