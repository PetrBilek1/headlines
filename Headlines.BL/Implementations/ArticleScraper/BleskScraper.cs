using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class BleskScraper : ArticleScraperBase
    {
        public BleskScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        public override async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                var title = GetTitle(document.DocumentNode);

                HtmlNode authorNode = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' social-container-bottom ')]//div[contains(concat(' ', @class, ' '), ' author ')]");
                authorNode ??= document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' author-container ')]");
                authorNode ??= document.DocumentNode.SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]");
                var author = GetAuthor(authorNode);

                var paragraphs = new List<string> { GetOpener(document.DocumentNode) };

                var isPagedArticle = document.DocumentNode.SelectSingleNode("//li[@id and @data-id]") != null;
                if (isPagedArticle)
                {
                    paragraphs.AddRange(GetPagedParagraphs(document.DocumentNode));
                }
                else
                {
                    HtmlNode contentNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' articleBody ') or contains(concat(' ', @class, ' '), ' content ')]");
                    paragraphs.AddRange(GetParagraphs(contentNode));
                }                

                HtmlNode tagsNode = document.DocumentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' tagsFooter ') or contains(concat(' ', @class, ' '), ' tags ')]");
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
            return node.SelectSingleNode(".//article//h1[text()]")?.InnerText.Trim() ?? string.Empty;
        }

        private string GetAuthor(HtmlNode node)
        {
            return node?.InnerText.Replace("Autor:", "").Replace("-", " ").Trim() ?? string.Empty;
        }

        private string GetOpener(HtmlNode node)
        {
            return node.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' leadsection ') or contains(concat(' ', @class, ' '), ' perex ')]/p[text()]")?
                .InnerText
                .Trim()
                ?? string.Empty;
        }

        private List<string> GetParagraphs(HtmlNode node)
        {
            return node.SelectNodes("./*[(self::p or self::h2) and not(contains(concat(' ', @class, ' '), ' title '))]")?
                .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }

        private List<string> GetPagedParagraphs(HtmlNode node)
        {
            return node.SelectNodes(string.Join(string.Empty, new string[]
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

        private List<string> GetTags(HtmlNode node)
        {
            return node.SelectNodes(".//a[text()]")?
                .Select(x => x.InnerText.Trim())
                .ToList()
                ?? new List<string>();
        }
    }
}