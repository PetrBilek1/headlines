using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Utils;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class IdnesScraper : IArticleScraper
    {
        private readonly IHtmlDocumentLoader _documentLoader;

        public IdnesScraper(IHtmlDocumentLoader documentLoader)
        {
            _documentLoader = documentLoader;
        }

        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {               
                HtmlDocument document = (await _documentLoader.LoadFromUrlAsync(url))
                    .ReplaceNewLineTags()
                    .Sanitize();

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]");
                var author = GetAuthor(authorNode);

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @itemprop, ' '), ' articleBody ')]");
                var paragraphs = new List<string> { GetOpener(document.DocumentNode) };

                HtmlNode? paywallNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @id, ' '), ' paywall ') ]");
                var isPaywalled = paywallNode != null;

                paragraphs.AddRange(GetParagraphs(contentNode));

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' art-tags ')]");
                var tags = GetTags(tagsNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = isPaywalled,
                    Title = title,
                    Author = author,
                    Paragraphs = paragraphs,
                    Tags = tags
                };
            }
            catch
            {
                return new ArticleScrapeResult { IsSuccess = false };
            }
        }

        private string GetTitle(HtmlNode node)
        {
            return node.SelectSingleNode("//h1[contains(concat(' ', @itemprop, ' '), ' headline ')]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return string.Join(", ", node.SelectNodes(".//span[contains(concat(' ', @itemprop, ' '), ' author ')]")?.Select(x => x.InnerText.Trim()) ?? new List<string>());
        }

        private string GetOpener(HtmlNode node)
        {
            return node.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' opener ')]")?.InnerText.Trim() ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//*[self::p or self::h3]")?
                .Where(x => !string.IsNullOrEmpty(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList() 
                ?? new List<string>();
        }

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//a[text()]")?
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }
    }
}