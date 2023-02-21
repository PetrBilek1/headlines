using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class BleskScraper : ArticleScraperBase
    {
        public BleskScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//article//h1[text()]")
                ?.InnerText.Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
        {
            var authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' social-container-bottom ')]//div[contains(concat(' ', @class, ' '), ' author ')]");
            
            authorNode ??= document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' author-container ')]");
            authorNode ??= document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]");

            return authorNode
                ?.InnerText
                .Replace("Autor:", "")
                .Replace("-", " ")
                .Trim() 
            ?? string.Empty;
        }

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' leadsection ') or contains(concat(' ', @class, ' '), ' perex ')]/p[text()]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
        {
            bool isPagedArticle = document.DocumentNode.SelectSingleNode("//li[@id and @data-id]") != null;
            if (isPagedArticle)
            {
                return document.DocumentNode
                    .SelectNodes(string.Join(string.Empty, new string[]
                    {
                        "//li[@id and @data-id and not(contains(concat(' ', @data-id, ' '), ' related '))]",
                        "//*[",
                        "self::p[not(ancestor::div[contains(concat(' ', @class, ' '), ' videoInArticle ')])]",
                        " or ",
                        "self::h2[not(@class) and text()]",
                        "]",
                    }))
                    .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    .Select(x => x.InnerText.Trim())
                    .ToList()
                ?? new List<string>();
            }

            var contentNode = document.DocumentNode
                .SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' articleBody ') or contains(concat(' ', @class, ' '), ' content ')]");

            return contentNode.SelectNodes("./*[(self::p or self::h2) and not(contains(concat(' ', @class, ' '), ' title '))]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
        }

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes(".//div[contains(concat(' ', @class, ' '), ' tagsFooter ') or contains(concat(' ', @class, ' '), ' tags ')]//a[text()]")
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}