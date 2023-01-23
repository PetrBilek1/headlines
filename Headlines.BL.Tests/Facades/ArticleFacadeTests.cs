using AutoMapper;
using FluentAssertions;
using Headlines.BL.DAO;
using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using Moq;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;
using Xunit;

namespace Headlines.BL.Tests.Facades
{
    public sealed class ArticleFacadeTests
    {
        private readonly ArticleFacade _sut;
        private readonly TestData _data;

        private readonly Mock<IArticleDAO> _articleDaoMock = new Mock<IArticleDAO>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWorkProvider> _uowProviderMock = new Mock<IUnitOfWorkProvider>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly IMapper _mapper = TestUtils.GetMapper();

        public ArticleFacadeTests()
        {
            _data = new TestData();
            _sut = new ArticleFacade(_articleDaoMock.Object, _uowProviderMock.Object, _mapper);
        }

        [Fact]
        public async Task GetArticlesByUrlIdsAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);

            _uowMock.Setup(x => x.Dispose());

            _articleDaoMock.Setup(x => x.GetByUrlIdsAsync(_data.UrlIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.Articles);

            //Act
            List<ArticleDTO> result = await _sut.GetArticlesByUrlIdsAsync(_data.UrlIds);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateArticleAsync_ShouldCreateArticle()
        {
            //Arrange
            ArticleDTO article = new()
            {
                Id = default,
                SourceId = 1,
                Published = new DateTime(2020, 10, 10),
                UrlId = "urlId",
                CurrentTitle = "title",
                Link = "link"
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _articleDaoMock.Setup(x => x.InsertAsync(It.IsAny<Article>()))
                .ReturnsAsync(new Article() { Id = 1 });

            //Act
            ArticleDTO result = await _sut.CreateOrUpdateArticleAsync(article);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _articleDaoMock.Verify(x => x.InsertAsync(It.IsAny<Article>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.SourceId.Should().Be(article.SourceId);
            result.Published.Should().Be(article.Published);
            result.UrlId.Should().Be(article.UrlId);
            result.CurrentTitle.Should().Be(article.CurrentTitle);
            result.Link.Should().Be(article.Link);
        }

        [Fact]
        public async Task CreateOrUpdateArticleAsync_ShouldUpdateArticle()
        {
            //Arrange
            ArticleDTO article = new()
            {
                Id = 1,
                SourceId = 1,
                Published = new DateTime(2022, 10, 10),
                UrlId = "NEWurlId",
                CurrentTitle = "NEWtitle",
                Link = "NEWlink"
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _articleDaoMock.Setup(x => x.AssertExistsAsync(article.Id))
                .ReturnsAsync(_data.Article1);

            //Act
            ArticleDTO result = await _sut.CreateOrUpdateArticleAsync(article);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _articleDaoMock.Verify(x => x.AssertExistsAsync(article.Id), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(article.Id);
            result.SourceId.Should().Be(article.SourceId);
            result.Published.Should().Be(article.Published);
            result.UrlId.Should().Be(article.UrlId);
            result.CurrentTitle.Should().Be(article.CurrentTitle);
            result.Link.Should().Be(article.Link);
        }

        private sealed class TestData
        {
            public string[] UrlIds => new[] { Article1.UrlId, Article2.UrlId };
            public List<Article> Articles => new() { Article1, Article2 };

            public Article Article1 = new()
            {
                Id = 1,
                SourceId = 1,
                Published = new DateTime(2020, 10, 10),
                UrlId = "urlId1",
                CurrentTitle = "title1",
                Link = "link1"
            };
            public Article Article2 = new()
            {
                Id = 2,
                SourceId = 1,
                Published = new DateTime(2020, 10, 11),
                UrlId = "urlId2",
                CurrentTitle = "title2",
                Link = "link2"
            };
        }
    }    
}