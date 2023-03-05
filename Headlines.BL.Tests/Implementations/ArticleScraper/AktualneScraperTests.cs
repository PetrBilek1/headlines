using Headlines.BL.Implementations.ArticleScraper;
using Headlines.BL.Tests.Resources.ScraperTestData;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class AktualneScraperTests : ScraperTestBase<AktualneScraper>
    {
        [Theory]
        [InlineData("001")]
        [InlineData("002")]
        [InlineData("003")]
        public async Task ScrapeArticleAsync(string index)
        {
            //Arrange
            var html = await ScraperDataLoader.GetHtmlAsync("Aktualne", index);
            var expected = await ScraperDataLoader.GetExpectedAsync("Aktualne", index);
            SetupDocumentLoader(html);

            //Act
            var result = await _sut.ScrapeArticleAsync(string.Empty);

            //Assert
            AssertResult(result, expected);
        }
    }
}