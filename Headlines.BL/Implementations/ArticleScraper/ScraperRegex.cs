using System.Text.RegularExpressions;

namespace Headlines.BL.Implementations.ArticleScraper
{
    public static partial class ScraperRegex
    {
        [GeneratedRegex(@"\s+")]
        public static partial Regex WhiteSpaceRegex();
    }
}