using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using Headlines.Enums;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class DeniknScraper : ArticleScraperBase
    {
        public override ArticleScraperType ScraperType { get; } = ArticleScraperType.DenikN;

        public DeniknScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("id", "e_lock__hard")}]") != null;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//h1[{ContainsExact("class", "e_title")}]")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "e_authors")}]//cite[text()]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .JoinStrings()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//div[{ContainsExact("class", "b_single_e")}]/p[text()]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//div[{ContainsExact("class", "a_single__post")}]//*[(self::p or self::h3) and not(ancestor::div[{ContainsExact("id", "e_lock__hard")}])]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();


        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//*[{ContainsExact("class", "e_terms_tag")}]")
                .SelectNotNullOrWhiteSpaceInnerText()
                .ToList();
    }
}