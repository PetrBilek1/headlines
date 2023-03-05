using HtmlAgilityPack;

namespace Headlines.BL.Abstractions.ArticleScraping
{
    public interface IHtmlDocumentSanitizer
    {
        HtmlDocument Sanitize(HtmlDocument document);
    }
}