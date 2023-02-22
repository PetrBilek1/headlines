using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

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
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
        {
            var authorNode = document.DocumentNode.SelectSingleNode($"//span[{ContainsExact("class", "article-info__author-name")}]");
            
            authorNode ??= document.DocumentNode.SelectSingleNode($"//a[{ContainsExact("class", "article-info__author")}]");

            return Regex.Replace(authorNode.InnerText.Replace("\n", "").Trim() ?? string.Empty, @"\s+", " ");
        }

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//p[{ContainsExact("class", "article-perex")}]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "article-text")}]//*[(self::p or self::h2) and text()]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "article-tags")}]//a[text()]")
                ?.Select(x => x.InnerText.Replace(",", "").Trim())
                .ToList()
            ?? new List<string>();
    }
}