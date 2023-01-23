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
    public sealed class ArticleSourceFacadeTests
    {
        private readonly ArticleSourceFacade _sut;
        private readonly TestData _data;

        private readonly Mock<IUnitOfWorkProvider> _uowProviderMock = new Mock<IUnitOfWorkProvider>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly Mock<IArticleSourceDAO> _articleSourceDaoMock = new Mock<IArticleSourceDAO>(MockBehavior.Strict);
        private readonly IMapper _mapper = TestUtils.GetMapper();

        public ArticleSourceFacadeTests()
        {
            _sut = new ArticleSourceFacade(_mapper, _uowProviderMock.Object, _articleSourceDaoMock.Object);
            _data = new TestData();
        }

        [Fact]
        public async Task GetAllArticleSourcesAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);

            _uowMock.Setup(x => x.Dispose());

            _articleSourceDaoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.ArticleSources);

            //Act
            List<ArticleSourceDTO> result = await _sut.GetAllArticleSourcesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result.Where(x => x.Id == 1).Should().HaveCount(1);
            result.First(x => x.Id == 1).Name.Should().Be(_data.ArticleSource1.Name);
            result.First(x => x.Id == 1).RssUrl.Should().Be(_data.ArticleSource1.RssUrl);
            result.First(x => x.Id == 1).UrlIdSource.Should().Be(_data.ArticleSource1.UrlIdSource);

            result.Where(x => x.Id == 2).Should().HaveCount(1);
            result.First(x => x.Id == 2).Name.Should().Be(_data.ArticleSource2.Name);
            result.First(x => x.Id == 2).RssUrl.Should().Be(_data.ArticleSource2.RssUrl);
            result.First(x => x.Id == 2).UrlIdSource.Should().Be(_data.ArticleSource2.UrlIdSource);

            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once());
        }

        private sealed class TestData
        {
            public List<ArticleSource> ArticleSources => new() { ArticleSource1, ArticleSource2 };

            public ArticleSource ArticleSource1 = new()
            {
                Id = 1,
                Name = "name1",
                RssUrl = "rssUrl1",
                UrlIdSource = Enums.ArticleUrlIdSource.Id
            };
            public ArticleSource ArticleSource2 = new()
            {
                Id = 2,
                Name = "name2",
                RssUrl = "rssUrl2",
                UrlIdSource = Enums.ArticleUrlIdSource.Link
            };
        }
    }    
}