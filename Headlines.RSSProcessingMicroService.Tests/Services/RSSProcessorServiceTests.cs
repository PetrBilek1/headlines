using FluentAssertions;
using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.RSSProcessingMicroService.DTO;
using Headlines.RSSProcessingMicroService.Services;
using Moq;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.RSSReaderService;
using Xunit;

namespace Headlines.RSSProcessingMicroService.Tests.Services
{
    public sealed class RSSProcessorServiceTests
    {
        private readonly RSSProcessorService _sut;

        private readonly Mock<IRSSSourceReaderService> _rssSourceReaderServiceMock = new Mock<IRSSSourceReaderService>(MockBehavior.Strict);
        private readonly Mock<IArticleFacade> _articleFacadeMock = new Mock<IArticleFacade>(MockBehavior.Strict);
        private readonly Mock<IHeadlineChangeFacade> _headlineChangeFacadeMock = new Mock<IHeadlineChangeFacade>(MockBehavior.Strict);
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        public RSSProcessorServiceTests()
        {
            _sut = new RSSProcessorService(_rssSourceReaderServiceMock.Object, _articleFacadeMock.Object, _headlineChangeFacadeMock.Object, _dateTimeProviderMock.Object);
        }

        [Theory]
        [InlineData(ArticleUrlIdSource.Id)]
        [InlineData(ArticleUrlIdSource.Link)]
        public async Task DoWorkAsync_ShouldCreateArticle_WhenArticleDoesNotExist(ArticleUrlIdSource urlIdSource)
        {
            //Arrange
            var data = new FeedItemWithArticle()
            {
                FeedItem = new FeedItemDTO()
                {
                    Id = "id",
                    Link = "link",
                    Published = new DateTime(2020, 10, 10),
                    Title = "title"
                },
                ArticleSource = new ArticleSourceDTO()
                {
                    Id = 1,
                    Name = "name",
                    RssUrl = "rssUrl",
                    UrlIdSource = urlIdSource
                },
                Article = null                
            };

            _rssSourceReaderServiceMock.Setup(x => x.ReadFeedItemsFromSourcesAsync(default))
                .ReturnsAsync(new List<FeedItemWithArticle> { data });
            _articleFacadeMock.Setup(x => x.CreateOrUpdateArticleAsync(It.IsAny<ArticleDTO>()))
                .ReturnsAsync((ArticleDTO article) => { return article; });

            //Act
            ProcessingResultDTO result = await _sut.DoWorkAsync();

            //Assert
            result.Should().NotBeNull();
            result.CreatedArticles.Should().HaveCount(1);
            result.UpdatedArticles.Should().HaveCount(0);

            ExpectedUrlId(data.FeedItem, urlIdSource).Should().Be(result.CreatedArticles[0].UrlId);

            result.CreatedArticles[0].SourceId.Should().Be(data.ArticleSource?.Id);
            result.CreatedArticles[0].CurrentTitle.Should().Be(data.FeedItem.Title);
            result.CreatedArticles[0].Published.Should().Be(data.FeedItem.Published);
            result.CreatedArticles[0].Link.Should().Be(data.FeedItem.Link);

            _articleFacadeMock.Verify(x => x.CreateOrUpdateArticleAsync(It.IsAny<ArticleDTO>()), Times.Once);
        }

        [Fact]
        public async Task DoWorkAsync_ShouldUpdateArticle_WhenArticleExists()
        {
            //Arrange
            var data = new FeedItemWithArticle()
            {
                FeedItem = new FeedItemDTO()
                {
                    Id = "id",
                    Link = "newLink",
                    Published = new DateTime(2020, 10, 10),
                    Title = "newTitle"
                },
                ArticleSource = new ArticleSourceDTO()
                {
                    Id = 1,
                    Name = "name",
                    RssUrl = "rssUrl",
                    UrlIdSource = ArticleUrlIdSource.Id
                },
                Article = new ArticleDTO()
                {
                    Id = 1,
                    SourceId = 1,
                    Published = new DateTime(2020, 10, 1),
                    UrlId = "id",
                    CurrentTitle = "oldTitle",
                    Link = "oldLink"
                }
            };

            _rssSourceReaderServiceMock.Setup(x => x.ReadFeedItemsFromSourcesAsync(default))
                .ReturnsAsync(new List<FeedItemWithArticle> { data });
            _articleFacadeMock.Setup(x => x.CreateOrUpdateArticleAsync(It.IsAny<ArticleDTO>()))
                .ReturnsAsync((ArticleDTO article) => { return article; });
            _headlineChangeFacadeMock.Setup(x => x.CreateOrUpdateHeadlineChangeAsync(It.IsAny<HeadlineChangeDTO>()))
                .ReturnsAsync(new HeadlineChangeDTO());
            _dateTimeProviderMock.Setup(x => x.Now)
                .Returns(new DateTime(2020, 10, 10));

            //Act
            ProcessingResultDTO result = await _sut.DoWorkAsync();

            //Assert
            result.Should().NotBeNull();
            result.CreatedArticles.Should().HaveCount(0);
            result.UpdatedArticles.Should().HaveCount(1);

            result.UpdatedArticles[0].Id.Should().Be(data.Article?.Id);
            result.UpdatedArticles[0].SourceId.Should().Be(data.Article?.SourceId);

            result.UpdatedArticles[0].Published.Should().Be(data.Article?.Published);
            result.UpdatedArticles[0].CurrentTitle.Should().Be(data.FeedItem?.Title);
            result.UpdatedArticles[0].Link.Should().Be(data.FeedItem?.Link);

            _articleFacadeMock.Verify(x => x.CreateOrUpdateArticleAsync(It.IsAny<ArticleDTO>()), Times.Once);
        }

        [Fact]
        public async Task DoWorkAsync_ShouldRecordHeadlineChange_Simple()
        {
            //Arrange
            var now = new DateTime(2020, 10, 15);
            var oldTitle = "oldTitle";

            var data = new FeedItemWithArticle()
            {
                FeedItem = new FeedItemDTO()
                {
                    Id = "id",
                    Link = "link",
                    Published = new DateTime(2020, 10, 10),
                    Title = "newTitle"
                },
                ArticleSource = new ArticleSourceDTO()
                {
                    Id = 1,
                    Name = "name",
                    RssUrl = "rssUrl",
                    UrlIdSource = ArticleUrlIdSource.Id
                },
                Article = new ArticleDTO()
                {
                    Id = 1,
                    SourceId = 1,
                    Published = new DateTime(2020, 10, 10),
                    UrlId = "id",
                    CurrentTitle = oldTitle,
                    Link = "link"
                }
            };            

            _rssSourceReaderServiceMock.Setup(x => x.ReadFeedItemsFromSourcesAsync(default))
                .ReturnsAsync(new List<FeedItemWithArticle> { data });
            _dateTimeProviderMock.Setup(x => x.Now)
                .Returns(now);
            _articleFacadeMock.Setup(x => x.CreateOrUpdateArticleAsync(It.IsAny<ArticleDTO>()))
                .ReturnsAsync((ArticleDTO article) => { return article; });
            _headlineChangeFacadeMock.Setup(x => x.CreateOrUpdateHeadlineChangeAsync(It.IsAny<HeadlineChangeDTO>()))
                .ReturnsAsync((HeadlineChangeDTO change) => { return change; });

            //Act
            ProcessingResultDTO result = await _sut.DoWorkAsync();

            //Assert
            result.Should().NotBeNull();
            result.RecordedHeadlineChanges.Should().HaveCount(1);

            result.RecordedHeadlineChanges[0].ArticleId.Should().Be(data.Article.Id);
            result.RecordedHeadlineChanges[0].Detected.Should().Be(now);
            result.RecordedHeadlineChanges[0].TitleBefore.Should().Be(oldTitle);
            result.RecordedHeadlineChanges[0].TitleAfter.Should().Be(data.FeedItem?.Title);
        }

        private string ExpectedUrlId(FeedItemDTO expectedFeedItem, ArticleUrlIdSource source)
        {
            return source switch
            {
                ArticleUrlIdSource.Id => expectedFeedItem.Id,
                ArticleUrlIdSource.Link => expectedFeedItem.Link,
                _ => throw new NotImplementedException(),
            };
        }
    }
}