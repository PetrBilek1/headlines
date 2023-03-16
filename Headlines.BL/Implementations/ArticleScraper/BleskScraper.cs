using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class BleskScraper : ArticleScraperBase
    {
        public BleskScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "subscription--article")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//article//h1[text()]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
        {
            var authorNode = document.DocumentNode.SelectSingleNode($"//div[{ContainsExact("class", "social-container-bottom")}]//div[{ContainsExact("class", "author")}]");
            
            authorNode ??= document.DocumentNode.SelectSingleNode($"//div[{ContainsExact("class", "author-container")}]");
            authorNode ??= document.DocumentNode.SelectSingleNode($"//div[{ContainsExact("class", "authors")}]");
            authorNode ??= document.DocumentNode.SelectSingleNode($"//div[{ContainsExact("class", "author")} and not(ancestor::div[{ContainsExact("class", "image-description")}])]");

            return authorNode
                ?.InnerText
                .Replace("Autor:", "")
                .Replace("-", " ")
                .Trim() 
            ?? string.Empty;
        }

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "leadsection")} or {ContainsExact("class", "perex")} or {ContainsExact("class", "articlePerex")}]/p[text()]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
        {
            bool isPagedArticle = document.DocumentNode.SelectSingleNode("//li[@id and @data-id]") != null;
            if (isPagedArticle)
            {
                return document.DocumentNode
                    .SelectNodes(string.Join(string.Empty,                     
                        $"//li[@id and @data-id and not({ContainsExact("data-id", "related")})]",
                        "//*[",
                        $"self::p[not(ancestor::div[{ContainsExact("class", "videoInArticle")}])]",
                        " or ",
                        "self::h2[not(@class) and text()]",
                        "]"
                    ))
                    ?.WhereNotInnerTextNullOrWhiteSpace()
                    .SelectInnerText()
                    .ToList()
                ?? new List<string>();
            }

            var contentNode = document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "articleBody")} or {ContainsExact("class", "content")} or {ContainsExact("class", "articleText")}]");

            return contentNode
                ?.SelectNodes($"./*[(self::p or self::h2) and not({ContainsExact("class", "title")})]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();
        }

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("class", "tagsFooter")} or {ContainsExact("class", "tags")}]//a[text()]")
                ?.SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}