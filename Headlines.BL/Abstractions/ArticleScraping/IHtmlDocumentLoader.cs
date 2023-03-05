using HtmlAgilityPack;

namespace Headlines.BL.Abstractions.ArticleScraping
{
    public interface IHtmlDocumentLoader
    {
        Task<HtmlDocument> LoadFromUrlAsync(string url);
    }
}