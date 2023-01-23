using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.RSSProcessingMicroService.Utils;
using PBilek.RSSReaderService;
using Xunit;

namespace Headlines.RSSProcessingMicroService.Tests.Utils
{
    public sealed class FeedItemUtilsTests
    {
        [Theory]
        [InlineData(ArticleUrlIdSource.Id)]
        [InlineData(ArticleUrlIdSource.Link)]
        public void GetUrlId_ShouldPickCorrectUrlId(ArticleUrlIdSource source)
        {
            //Arrange
            FeedItemDTO feedItem = new()
            {
                Id = "id",
                Link = "link",
                Published = new DateTime(2020, 10, 10),
                Title = "title"
            };

            ArticleSourceDTO articleSource = new()
            {
                Id = 1,
                Name = "name",
                RssUrl = "url",
                UrlIdSource = source,
            };

            //Act
            string result = FeedItemUtils.GetUrlId(feedItem, articleSource);

            //Assert
            result.Should().Be(source switch
            {
                ArticleUrlIdSource.Id => feedItem.Id,
                ArticleUrlIdSource.Link => feedItem.Link,
                _ => throw new NotImplementedException()
            });
        }

        [Fact]
        public void GetUrlId_ShouldThrowNotImplementedException_WhenPassedNotImplementedEnum()
        {
            //Arrange
            ArticleUrlIdSource notImplementedEnum = (ArticleUrlIdSource)9999999;

            FeedItemDTO feedItem = new()
            {
                Id = "id",
                Link = "link",
                Published = new DateTime(2020, 10, 10),
                Title = "title"
            };

            ArticleSourceDTO articleSource = new()
            {
                Id = 1,
                Name = "name",
                RssUrl = "url",
                UrlIdSource = notImplementedEnum,
            };

            //Act
            Action act = () => FeedItemUtils.GetUrlId(feedItem, articleSource);

            //Assert
            act.Should().Throw<NotImplementedException>().WithMessage($"{nameof(articleSource.UrlIdSource)} is not implemented.");
        }
    }
}