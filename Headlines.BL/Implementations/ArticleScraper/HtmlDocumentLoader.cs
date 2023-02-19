using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HtmlDocumentLoader : IHtmlDocumentLoader
    {
        private readonly IHtmlDocumentSanitizer _sanitizer;
        private readonly IHttpClientFactory _httpClientFactory;

        public HtmlDocumentLoader(IHtmlDocumentSanitizer sanitizer, IHttpClientFactory httpClientFactory)
        {
            _sanitizer = sanitizer;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HtmlDocument> LoadFromUrlAsync(string url)
        {
            var client = _httpClientFactory.CreateClient();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.TryAddWithoutValidation("ACCEPT", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            requestMessage.Headers.TryAddWithoutValidation("ACCEPT-LANGUAGE", "cs-CZ,cs;q=0.5");
            requestMessage.Headers.TryAddWithoutValidation("REFERER", "https://www.google.com/");
            requestMessage.Headers.TryAddWithoutValidation("USER-AGENT", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            requestMessage.Headers.TryAddWithoutValidation("SEC-FETCH-DEST", "document");
            requestMessage.Headers.TryAddWithoutValidation("SEC-FETCH-MODE", "navigate");
            requestMessage.Headers.TryAddWithoutValidation("SEC-FETCH-SITE", "cross-site");
            requestMessage.Headers.TryAddWithoutValidation("SEC-FETCH-USER", "?1");
            requestMessage.Headers.TryAddWithoutValidation("SEC-GPC", "1");
            requestMessage.Headers.TryAddWithoutValidation("UPGRADE-INSECURE-REQUESTS", "1");

            var response = await client.SendAsync(requestMessage);
            var html = await response.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var sanitizedDocument = _sanitizer.Sanitize(document);

            return sanitizedDocument;
        }
    }
}