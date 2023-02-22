using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class LidovkyScraper : ArticleScraperBase
    {
        public LidovkyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("id", "paywall")}]")
                != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//h1[{ContainsExact("itemprop", "headline")}]")
                ?.InnerText
                .Trim() 
            ?? string.Empty;
       

        protected override string GetAuthor(HtmlDocument document)
            => string.Join(
                ", ",
                document.DocumentNode
                    .SelectNodes($".//span[{ContainsExact("itemprop", "author")}]")
                    .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                    .Select(x => x.InnerText.Trim())
                    .Distinct()
                );

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "opener")}]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("itemprop", "articleBody")}]//*[self::p or self::h3]")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "art-tags")}]/a[text()]")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}