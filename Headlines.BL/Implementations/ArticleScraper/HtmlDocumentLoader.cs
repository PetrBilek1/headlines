using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HtmlDocumentLoader : IHtmlDocumentLoader
    {
        public Task<HtmlDocument> LoadFromUrlAsync(string url) => new HtmlWeb().LoadFromWebAsync(url);
    }
}