using FluentAssertions;
using Headlines.BL.Implementations.ArticleScraper;
using Headlines.BL.Tests.Resources.ScraperTestData;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class IrozhlasScraperTests : ScraperTestBase<IrozhlasScraper>
    {
        [Theory]
        [InlineData("001")]
        public async Task ScrapeArticleAsync(string index)
        {
            //Arrange
            var html = await ScraperDataLoader.GetHtmlAsync("Irozhlas", index);
            var expected = await ScraperDataLoader.GetExpectedAsync("Irozhlas", index);
            SetupDocumentLoader(html);

            //Act
            var result = await _sut.ScrapeArticleAsync(string.Empty);

            //Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(expected.IsSuccess);
            result.IsPaywalled.Should().Be(expected.IsPaywalled);
            result.Title.Should().Be(expected.Title);
            result.Author.Should().Be(expected.Author);

            result.Paragraphs.Should().HaveCount(expected.Paragraphs.Count);
            for (int i = 0; i < expected.Paragraphs.Count; i++)
            {
                result.Paragraphs[i].Should().Be(expected.Paragraphs[i]);
            }

            result.Tags.Should().HaveCount(expected.Tags.Count);
            for (int i = 0; i < expected.Tags.Count; i++)
            {
                result.Tags[i].Should().Be(expected.Tags[i]);
            }
        }
    }
}