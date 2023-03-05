using Headlines.BL.Implementations.ArticleScraper;
using Headlines.BL.Tests.Resources.ScraperTestData;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class A2larmScraperTests : ScraperTestBase<A2larmScraper>
    {
        [Theory]
        [InlineData("001")]
        [InlineData("002")]
        public async Task ScrapeArticleAsync(string index)
        {
            //Arrange
            var html = await ScraperDataLoader.GetHtmlAsync("A2larm", index);
            var expected = await ScraperDataLoader.GetExpectedAsync("A2larm", index);
            SetupDocumentLoader(html);

            //Act
            var result = await _sut.ScrapeArticleAsync(string.Empty);

            //Assert
            AssertResult(result, expected);
        }
    }
}