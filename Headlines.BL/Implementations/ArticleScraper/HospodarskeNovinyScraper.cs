﻿using Headlines.BL.Abstractions.ArticleScraping;
using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public sealed class HospodarskeNovinyScraper : ArticleScraperBase
    {
        public HospodarskeNovinyScraper(IHtmlDocumentLoader documentLoader) : base(documentLoader)
        {
        }

        protected override bool IsPaywalled(HtmlDocument document)
            => document.DocumentNode.SelectNodes(".//div[contains(concat(' ', @class, ' '), ' paywall ')]")
                ?.Any() == true;

        protected override string GetTitle(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//h1")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetAuthor(HtmlDocument document)
            => document.DocumentNode
                .SelectSingleNode("//div[contains(concat(' ', @class, ' '), ' authors ')]/a[text()]")
                ?.InnerText
                .Trim()
            ?? string.Empty;

        protected override string GetPerex(HtmlDocument document) => string.Empty;

        protected override List<string> GetParagraphs(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes(".//div[contains(concat(' ', @class, ' '), ' article-body-part ') and contains(concat(' ', @class, ' '), ' free-part ')]//p")
                ?.Select(x => x.InnerText)
                .ToList()
            ?? new List<string>();

        protected override List<string> GetTags(HtmlDocument document)
            => document.DocumentNode
                .SelectNodes(".//div[contains(concat(' ', @class, ' '), ' tags ')]//li[contains(@id, 'tag')]")
                ?.Select(x => x.InnerText.Trim())
                .ToList()
            ?? new List<string>();
    }
}