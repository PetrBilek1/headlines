using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper.Extensions
{
    internal static class ArticleScraperExtensions
    {
        public static string JoinStrings(this IEnumerable<string>? strings, string joinBy = ", ")
            => string.Join(joinBy, strings ?? Enumerable.Empty<string>());

        public static IEnumerable<string> SelectNotNullOrWhiteSpaceInnerText(this HtmlNodeCollection nodes)
            => nodes
                .WhereInnerTextNotNullOrWhiteSpace()
                .SelectInnerText();

        public static IEnumerable<HtmlNode> WhereInnerTextNotNullOrWhiteSpace(this HtmlNodeCollection nodes)
            => nodes.Where(x => !string.IsNullOrWhiteSpace(x.InnerText));

        public static string SelectInnerText(this HtmlNode node)
            => node?.InnerText.Trim() ?? string.Empty;

        public static IEnumerable<string> SelectInnerText(this IEnumerable<HtmlNode> nodes)
            => nodes
                ?.Select(x => x.InnerText.Trim())
            ?? Enumerable.Empty<string>();

        public static IEnumerable<string> ReplaceLongWhiteSpaces(this IEnumerable<string> strings, string replaceBy = " ")
            => strings.Select(x => ScraperRegex.WhiteSpaceRegex().Replace(x, replaceBy));
    }
}