using System.Text.RegularExpressions;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public static partial class ScraperRegex
    {
        [GeneratedRegex(@"\s+", RegexOptions.NonBacktracking, 100)]
        public static partial Regex WhiteSpaceRegex();
    }
}