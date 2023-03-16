using HtmlAgilityPack;

namespace Headlines.BL.Implementations.ArticleScraper.Extensions
{
    internal static class ArticleScraperExtensions
    {
        public static string JoinStrings(this IEnumerable<string>? strings, string joinBy = ", ")
            => string.Join(joinBy, strings ?? Enumerable.Empty<string>());

        public static IEnumerable<string> SelectNotNullOrWhiteSpaceInnerText(this HtmlNodeCollection nodes)
            => nodes
                .WhereNotInnerTextNullOrWhiteSpace()
                .SelectInnerText();

        public static IEnumerable<HtmlNode> WhereNotInnerTextNullOrWhiteSpace(this HtmlNodeCollection nodes)
            => nodes.Where(x => !string.IsNullOrWhiteSpace(x.InnerText));        

        public static IEnumerable<string> SelectInnerText(this IEnumerable<HtmlNode> nodes, bool trim = true)
            => nodes.Select(x => SelectInnerText(x, trim));

        public static string SelectInnerText(this HtmlNode? node, bool trim = true)
            => node?.InnerText.Trim() ?? string.Empty;

        public static IEnumerable<string> ReplaceLongWhiteSpaces(this IEnumerable<string> strings, string replaceBy = " ")
            => strings.Select(x => ReplaceLongWhiteSpaces(x));

        public static string ReplaceLongWhiteSpaces(this string text, string replaceBy = " ")
            => ScraperRegex.WhiteSpaceRegex().Replace(text, replaceBy);
    }
}