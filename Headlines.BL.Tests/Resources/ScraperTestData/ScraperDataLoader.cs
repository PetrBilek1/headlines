using Headlines.BL.Abstractions.ArticleScraping;
using Newtonsoft.Json;

namespace Headlines.BL.Tests.Resources.ScraperTestData
{
    internal static class ScraperDataLoader
    {
        internal static async Task<string> GetHtmlAsync(string folder, string index)
        {
            using StreamReader reader = new StreamReader(GetPath(folder, index, "input"));            

            return await reader.ReadToEndAsync();
        }

        internal static async Task<ArticleScrapeResult> GetExpectedAsync(string folder, string index)
        {
            using StreamReader reader = new StreamReader(GetPath(folder, index, "expected"));

            var expected = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<ArticleScrapeResult>(expected) ?? throw new Exception();
        }

        private static string GetPath(string folder, string index, string fileType)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "ScraperTestData",
                folder,
                $"{index}.{fileType}"
                );
        }
    }
}