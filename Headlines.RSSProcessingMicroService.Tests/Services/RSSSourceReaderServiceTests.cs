using FluentAssertions;
using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.RSSProcessingMicroService.DTO;
using Headlines.RSSProcessingMicroService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using PBilek.RSSReaderService;
using Xunit;

namespace Headlines.RSSProcessingMicroService.Tests.Services
{
    public sealed class RSSSourceReaderServiceTests
    {
        private readonly RssSourceReaderService _sut;
        private readonly TestData _data;

        private readonly Mock<IRSSReaderService> _rssReaderServiceMock = new Mock<IRSSReaderService>(MockBehavior.Strict);
        private readonly Mock<IArticleSourceFacade> _articleSourceFacadeMock = new Mock<IArticleSourceFacade>(MockBehavior.Strict);
        private readonly Mock<IArticleFacade> _articleFacadeMock = new Mock<IArticleFacade>(MockBehavior.Strict);
        private readonly Mock<ILogger<RssSourceReaderService>> _loggerMock = new Mock<ILogger<RssSourceReaderService>>();

        public RSSSourceReaderServiceTests()
        {
            _sut = new RssSourceReaderService(_rssReaderServiceMock.Object, _articleSourceFacadeMock.Object, _articleFacadeMock.Object, _loggerMock.Object);
            _data = new TestData();
        }

        [Fact]
        public async Task ReadFeedItemsFromSourcesAsync_ShouldReturnList_Simple()
        {
            //Arrange           
            _articleSourceFacadeMock.Setup(x => x.GetAllArticleSourcesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.ArticleSources);

            _rssReaderServiceMock.Setup(x => x.ReadFeedItemsAsync(_data.ArticleSources[0].RssUrl, default))
                .ReturnsAsync(_data.FeedItems1);
            _rssReaderServiceMock.Setup(x => x.ReadFeedItemsAsync(_data.ArticleSources[1].RssUrl, default))
                .ReturnsAsync(_data.FeedItems2);

            _articleFacadeMock.Setup(x => x.GetArticlesByUrlIdsAsync(new string[] { "id1", "id2" }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.Articles1);
            _articleFacadeMock.Setup(x => x.GetArticlesByUrlIdsAsync(new string[] { "link3" }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.Articles2);

            //Act            
            List<FeedItemWithArticle> result = await _sut.ReadFeedItemsFromSourcesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            List<FeedItemWithArticle> resultOfSource1 = result.Where(x => x.ArticleSource == _data.ArticleSource1).ToList();
            List<FeedItemWithArticle> resultOfSource2 = result.Where(x => x.ArticleSource == _data.ArticleSource2).ToList();

            resultOfSource1.Should().HaveCount(2);
            resultOfSource2.Should().HaveCount(1);

            resultOfSource1.Where(x => x.Article == _data.Article1).Should().HaveCount(1);
            resultOfSource1.First(x => x.Article == _data.Article1).FeedItem.Should().NotBeNull();
            resultOfSource1.First(x => x.Article == _data.Article1).ArticleSource.Should().NotBeNull();
            resultOfSource1.First(x => x.Article == _data.Article1).Article.Should().NotBeNull();
            resultOfSource1.First(x => x.Article == _data.Article1).FeedItem.Should().Be(_data.FeedItem1);

            resultOfSource1.Where(x => x.Article == null).Should().HaveCount(1);
            resultOfSource1.First(x => x.Article == null).FeedItem.Should().NotBeNull();
            resultOfSource1.First(x => x.Article == null).ArticleSource.Should().NotBeNull();
            resultOfSource1.First(x => x.Article == null).FeedItem.Should().Be(_data.FeedItem2);
            resultOfSource1.First(x => x.Article == null).Article.Should().BeNull();

            resultOfSource2.Where(x => x.Article == _data.Article2).Should().HaveCount(1);
            resultOfSource2.First(x => x.Article == _data.Article2).FeedItem.Should().NotBeNull();
            resultOfSource2.First(x => x.Article == _data.Article2).ArticleSource.Should().NotBeNull();
            resultOfSource2.First(x => x.Article == _data.Article2).Article.Should().NotBeNull();
            resultOfSource2.First(x => x.Article == _data.Article2).FeedItem.Should().Be(_data.FeedItem3);
        }

        [Fact]
        public async Task ReadFeedItemsFromSourcesAsync_ShouldLogInformation_WhenCouldNotReadFeed()
        {
            //Arrange
            _articleSourceFacadeMock.Setup(x => x.GetAllArticleSourcesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.ArticleSources);

            _rssReaderServiceMock.Setup(x => x.ReadFeedItemsAsync(It.IsAny<string>(), default))
                .Throws<Exception>();

            //Act
            List<FeedItemWithArticle> result = await _sut.ReadFeedItemsFromSourcesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _loggerMock.VerifyLog(x => x.LogInformation("Could not read RSS feed of source '{name}' with exception '{message}'.", It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }

    sealed class TestData
    {
        public List<ArticleSourceDto> ArticleSources => new List<ArticleSourceDto>() { ArticleSource1, ArticleSource2 };
        public List<FeedItemDTO> FeedItems1 => new List<FeedItemDTO>() { FeedItem1, FeedItem2 };
        public List<FeedItemDTO> FeedItems2 => new List<FeedItemDTO>() { FeedItem3 };
        public List<ArticleDto> Articles1 => new List<ArticleDto>() { Article1 };   
        public List<ArticleDto> Articles2 => new List<ArticleDto>() { Article2 };

        public ArticleSourceDto ArticleSource1 = new()
        {
            Id = 1,
            Name = "name",
            RssUrl = "rssUrl",
            UrlIdSource = Enums.ArticleUrlIdSource.Id
        };
        public ArticleSourceDto ArticleSource2 = new()
        {
            Id = 2,
            Name = "name2",
            RssUrl = "rssUrl2",
            UrlIdSource = Enums.ArticleUrlIdSource.Link
        };

        public FeedItemDTO FeedItem1 = new()
        {
            Id = "id1",
            Link = "link1",
            Published = new DateTime(2020, 10, 10),
            Title = "title1"
        };
        public FeedItemDTO FeedItem2 = new()
        {
            Id = "id2",
            Link = "link2",
            Published = new DateTime(2020, 10, 11),
            Title = "title2"
        };
        public FeedItemDTO FeedItem3 = new()
        {
            Id = null!,
            Link = "link3",
            Published = new DateTime(2020, 10, 12),
            Title = "title3"
        };

        public ArticleDto Article1 = new()
        {
            Id = 1,
            SourceId = 1,
            Published = new DateTime(2020, 10, 10),
            UrlId = "id1",
            CurrentTitle = "title1",
            Link = "link1"
        };
        public ArticleDto Article2 = new()
        {
            Id = 2,
            SourceId = 2,
            Published = new DateTime(2020, 10, 10),
            UrlId = "link3",
            CurrentTitle = "title3",
            Link = "link3"
        };
    }
}