using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DenikczScraper : ArticleScraperBase
    {
        public DenikczScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//span[contains(concat(' ', @class, ' '), ' article-info__author-name ')]");
                var author = GetAuthor(authorNode);

                var paragraphs = new List<string> { GetOpener(document.DocumentNode) };

                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' article-text ')]");
                paragraphs.AddRange(GetParagraphs(contentNode));

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' article-tags ')]");
                var tags = GetTags(tagsNode);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = false,
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
            return node.SelectSingleNode(".//h1[contains(concat(' ', @class, ' '), ' article-title ')]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return Regex.Replace(node.InnerText.Replace("\n", "").Trim() ?? string.Empty, @"\s+", " ");
        }

        private string GetOpener(HtmlNode node)
        {
            return node.SelectSingleNode(".//p[contains(concat(' ', @class, ' '), ' article-perex ')]")?
                .InnerText
                .Trim()
                ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes(".//*[(self::p or self::h2) and text()]")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//a[text()]")?
                .Select(x => x.InnerText.Replace(",", "").Trim())                
                .ToList()
                ?? new List<string>();
        }
    }
}