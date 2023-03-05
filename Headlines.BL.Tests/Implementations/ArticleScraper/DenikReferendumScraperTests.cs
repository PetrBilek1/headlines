using Headlines.BL.Implementations.ArticleScraper;
using Headlines.BL.Tests.Resources.ScraperTestData;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class DenikReferendumScraperTests : ScraperTestBase<DenikReferendumScraper>
    {
        [Theory]
        [InlineData("001")]
        [InlineData("002")]
        public async Task ScrapeArticleAsync(string index)
        {
            //Arrange
            var html = await ScraperDataLoader.GetHtmlAsync("DenikReferendum", index);
            var expected = await ScraperDataLoader.GetExpectedAsync("DenikReferendum", index);
            SetupDocumentLoader(html);

            //Act
            var result = await _sut.ScrapeArticleAsync(string.Empty);

            //Assert
            AssertResult(result, expected);
        }
    }
}