using Ganss.Xss;
using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HtmlDocumentSanitizer : IHtmlDocumentSanitizer
    {
        public HtmlDocument Sanitize(HtmlDocument document)
        {
            var outputDocument = ReplaceNewLineTags(document);

            outputDocument = RemoveProhibitedNodes(outputDocument);

            return outputDocument;
        }

        private HtmlDocument ReplaceNewLineTags(HtmlDocument inputDocument, string replaceWith = "\n")
        {
            var html = inputDocument.DocumentNode.OuterHtml
                .Replace("<br>", replaceWith)
                .Replace("</br>", replaceWith);

            var outputDocument = new HtmlDocument();
            outputDocument.LoadHtml(html);

            return outputDocument;
        }

        private HtmlDocument RemoveProhibitedNodes(HtmlDocument inputDocument)
        {
            var sanitizer = new HtmlSanitizer(new HtmlSanitizerOptions
            {
                AllowedTags =
                {
                    "a",
                    "article",
                    "b",
                    "blockquote",
                    "cite",
                    "div",
                    "em",
                    "footer",
                    "h1",
                    "h2",
                    "h3",
                    "header",
                    "i",
                    "li",
                    "main",
                    "nav",
                    "p",
                    "section",
                    "span",
                    "strong",
                    "ul"
                },
                AllowedAttributes =
                {
                    "class",
                    "data-dot",
                    "data-id",
                    "href",
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