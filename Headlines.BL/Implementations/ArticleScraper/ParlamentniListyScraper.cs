﻿using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper.Extensions;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class ParlamentniListyScraper : ArticleScraperBase
    {
        public ParlamentniListyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document) => false;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//section[{ContainsExact("class", "article-header")}]/h1")
                .SelectInnerText();

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//section[{ContainsExact("class", "section-inarticle")}]/p/i")
                ?.SelectInnerText(false)
                .Replace("autor:", "")
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode($"//section[{ContainsExact("class", "article-header")}]/p[{ContainsExact("class", "brief")}]")
                .SelectInnerText();

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//section[{ContainsExact("class", "article-content")}]/p[not(descendant::a[contains(@href, 'https://www.parlamentnilisty.cz/predplatne') or contains(@href, 'https://www.parlamentnilisty.cz/profily-sprava/ProfileRegistration.aspx')])]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes($"//section[{ContainsExact("class", "article-tags")}]//*[{ContainsExact("class", "tag")}]")
                ?.WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText()
                .ToList()
            ?? new List<string>();
    }
}