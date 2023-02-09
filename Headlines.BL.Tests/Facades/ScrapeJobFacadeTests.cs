using AutoMapper;
using FluentAssertions;
using Headlines.BL.DAO;
using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.ORM.Core.Entities;
using Moq;
using PBilek.ORM.Core.UnitOfWork;
using Xunit;

namespace Headlines.BL.Tests.Facades
{
    public sealed class ScrapeJobFacadeTests
    {
        private readonly ScrapeJobFacade _sut;
        private readonly TestData _data;

        private readonly Mock<IScrapeJobDAO> _scrapeJobDAOMock = new Mock<IScrapeJobDAO>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWorkProvider> _uowProviderMock = new Mock<IUnitOfWorkProvider>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly IMapper _mapper = TestUtils.GetMapper();

        public ScrapeJobFacadeTests()
        {
            _sut = new ScrapeJobFacade(_uowProviderMock.Object, _scrapeJobDAOMock.Object, _mapper);
            _data = new TestData();
        }

        [Fact]
        public async Task CreateOrUpdateArticleAsync_ShouldCreateArticle()
        {
            //Arrange
            ScrapeJobDTO scrapeJob = new()
            {
                Id = default,
                ArticleId = 1,
                Priority = ScrapeJobPriority.Normal
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _scrapeJobDAOMock.Setup(x => x.InsertAsync(It.IsAny<ScrapeJob>()))
                .ReturnsAsync(new ScrapeJob() { Id = 1 });

            //Act
            ScrapeJobDTO result = await _sut.CreateOrUpdateScrapeJobAsync(scrapeJob);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _scrapeJobDAOMock.Verify(x => x.InsertAsync(It.IsAny<ScrapeJob>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ArticleId.Should().Be(1);
            result.Priority.Should().Be(ScrapeJobPriority.Normal);
        }

        [Fact]
        public async Task CreateOrUpdateArticleAsync_ShouldUpdateArticle()
        {
            //Arrange
            ScrapeJobDTO scrapeJob = new()
            {
                Id = 1,
                ArticleId = 2,
                Priority = ScrapeJobPriority.Lowest
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _scrapeJobDAOMock.Setup(x => x.AssertExistsAsync(scrapeJob.Id))
                .ReturnsAsync(_data.ScrapeJob1);

            //Act
            ScrapeJobDTO result = await _sut.CreateOrUpdateScrapeJobAsync(scrapeJob);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _scrapeJobDAOMock.Verify(x => x.AssertExistsAsync(scrapeJob.Id), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(scrapeJob.Id);
            result.ArticleId.Should().Be(scrapeJob.ArticleId);
            result.Priority.Should().Be(scrapeJob.Priority);
        }

        private sealed class TestData
        {
            public List<ScrapeJob> ScrapeJobs => new() { ScrapeJob1, ScrapeJob2 };

            public ScrapeJob ScrapeJob1 = new()
            {
                Id = 1,
                ArticleId = 1,
                Priority = ScrapeJobPriority.Normal
            };
            public ScrapeJob ScrapeJob2 = new()
            {
                Id = 2,
                ArticleId = 2,
                Priority = ScrapeJobPriority.Lowest
            };
        }
    }
}