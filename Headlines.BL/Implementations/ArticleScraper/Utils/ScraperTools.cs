using Ganss.Xss;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper.Utils
{
    internal static class ScraperTools
    {
        internal static HtmlDocument ReplaceNewLineTags(this HtmlDocument inputDocument, string replaceWith = "\n")
        {
            var html = inputDocument.DocumentNode.OuterHtml
                .Replace("<br>", replaceWith)
                .Replace("</br>", replaceWith);

            var outputDocument = new HtmlDocument();
            outputDocument.LoadHtml(html);

            return outputDocument;
        }

        internal static HtmlDocument Sanitize(this HtmlDocument inputDocument)
        {
            var sanitizer = new HtmlSanitizer(new HtmlSanitizerOptions
            {
                AllowedTags =
                {
                    "a",
                    "article",
                    "b",
                    "div",
                    "h1",
                    "h3",
                    "header",
                    "li",
                    "main",
                    "p",
                    "span",
                    "strong",
                    "ul"
                },
                AllowedAttributes =
                {
                    "class",
                    "id",
                    "itemprop"
                }
            });

            var html = sanitizer
                .Sanitize(inputDocument.DocumentNode.OuterHtml)
                .Replace("&nbsp;", " ");

            var outputDocument = new HtmlDocument();
            outputDocument.LoadHtml(html);         

            return outputDocument;
        }
    }
}