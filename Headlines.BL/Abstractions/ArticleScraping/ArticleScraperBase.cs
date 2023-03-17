using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Abstractions.ArticleScraping
{
    public abstract class ArticleScraperBase : IArticleScraper
    {
        protected IHtmlDocumentLoader _documentLoader;

        protected ArticleScraperBase(IHtmlDocumentLoader documentLoader)
        {
            _documentLoader = documentLoader;
        }

        public async Task<ArticleScrapeResult> ScrapeArticleAsync(string url)
        {
            try
            {
                HtmlDocument document = await _documentLoader.LoadFromUrlAsync(url);

                return new ArticleScrapeResult
                {
                    IsSuccess = true,
                    IsPaywalled = IsPaywalled(document),
                    Title = GetTitle(document),
                    Author = GetAuthor(document),
                    Paragraphs = MergePerexAndParagraphs(GetPerex(document), GetParagraphs(document)),
                    Tags = GetTags(document),
                };
            }
            catch
            {
                return new ArticleScrapeResult { IsSuccess = false };
            }
        }

        public abstract ArticleScraperType ScraperType { get; }

        protected abstract bool IsPaywalled(HtmlDocument document);
        protected abstract string GetTitle(HtmlDocument document);
        protected abstract string GetAuthor(HtmlDocument document);
        protected abstract string GetPerex(HtmlDocument document);
        protected abstract List<string> GetParagraphs(HtmlDocument document);
        protected abstract List<string> GetTags(HtmlDocument document);

        protected static string ContainsExact(string attributeName, string value)
            => $"contains(concat(' ', normalize-space(@{attributeName}), ' '), ' {value} ')";

        private static List<string> MergePerexAndParagraphs(string perex, List<string> paragraphs)
        {
            var merged = new List<string>(paragraphs.Count + 1);

            if (!string.IsNullOrEmpty(perex))
            {
                merged.Add(perex);
            }
            merged.AddRange(paragraphs);

            return merged;
        }
    }
}