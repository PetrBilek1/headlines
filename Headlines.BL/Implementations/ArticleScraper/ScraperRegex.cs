using System.Text.RegularExpressions;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public static partial class ScraperRegex
    {
        [GeneratedRegex(@"\s+", RegexOptions.None, 100)]
        public static partial Regex WhiteSpaceRegex();
    }
}