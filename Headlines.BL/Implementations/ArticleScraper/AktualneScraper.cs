using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class AktualneScraper : ArticleScraperBase
    {
        public AktualneScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1")
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "author")}]//*[{ContainsExact("class", "author__name")}]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($".//div[{ContainsExact("class", "article__perex")}]")
                ?.InnerText
                .Trim() 
            ?? string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
        {
            var contentNode = document.DocumentNode.SelectSingleNode($".//div[{ContainsExact("class", "article__content")}]");
            
            contentNode ??= document.DocumentNode.SelectSingleNode($".//div[{ContainsExact("class", "article")}]/div[{ContainsExact("id", "root")}]");

            return contentNode.SelectNodes(".//p")
                ?.Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
                .Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
        }

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($".//div[{ContainsExact("class", "taglist")}]//a[text()]")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}