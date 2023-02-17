using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HtmlDocumentLoader : IHtmlDocumentLoader
    {
        private readonly IHtmlDocumentSanitizer _sanitizer;

        public HtmlDocumentLoader(IHtmlDocumentSanitizer sanitizer)
        {
            _sanitizer = sanitizer;
        }

        public async Task<HtmlDocument> LoadFromUrlAsync(string url)
        {
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync(url);

            var sanitizedDocument = _sanitizer.Sanitize(document);

            return sanitizedDocument;
        }
    }
}