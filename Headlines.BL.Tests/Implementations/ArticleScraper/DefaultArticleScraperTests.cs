using FluentAssertions;
using Headlines.BL.Exceptions;
using Headlines.BL.Implementations.ArticleScraper;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class DefaultArticleScraperTests
    {
        private readonly DefaultArticleScraper _sut = new();

        [Fact]
        public async Task ScrapeArticleAsync()
        {
            //Act
            Func<Task> act = async () => await _sut.ScrapeArticleAsync("url");

            //Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }
    }
}