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
    public sealed class HeadlineChangeFacadeTests
    {
        private readonly HeadlineChangeFacade _sut;
        private readonly TestData _data;

        private readonly Mock<IUnitOfWorkProvider> _uowProviderMock = new Mock<IUnitOfWorkProvider>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly Mock<IHeadlineChangeDAO> _headlineChangeDaoMock = new Mock<IHeadlineChangeDAO>(MockBehavior.Strict);
        private readonly IMapper _mapper = TestUtils.GetMapper();

        public HeadlineChangeFacadeTests()
        {
            _sut = new HeadlineChangeFacade(_uowProviderMock.Object, _headlineChangeDaoMock.Object, _mapper);
            _data = new TestData();
        }

        [Fact]
        public async Task CreateOrUpdateHeadlineChangeAsync_ShouldCreateHeadlineChange()
        {
            //Arrange
            HeadlineChangeDTO headlineChange = new()
            {
                Id = default,
                ArticleId = 1,
                Detected = new DateTime(2020, 10, 10),
                TitleBefore = "titleBefore",
                TitleAfter = "titleAfter",
                UpvoteCount = 100
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _headlineChangeDaoMock.Setup(x => x.InsertAsync(It.IsAny<HeadlineChange>()))
                .ReturnsAsync(new HeadlineChange() { Id = 1 });

            //Act
            HeadlineChangeDTO result = await _sut.CreateOrUpdateHeadlineChangeAsync(headlineChange);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _headlineChangeDaoMock.Verify(x => x.InsertAsync(It.IsAny<HeadlineChange>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().NotBe(default);
            result.ArticleId.Should().Be(headlineChange.ArticleId);
            result.Detected.Should().Be(headlineChange.Detected);
            result.TitleBefore.Should().Be(headlineChange.TitleBefore);
            result.TitleAfter.Should().Be(headlineChange.TitleAfter);
            result.UpvoteCount.Should().Be(headlineChange.UpvoteCount);
        }

        [Fact]
        public async Task CreateOrUpdateHeadlineChangeAsync_ShouldUpdateHeadlineChange()
        {
            //Arrange
            HeadlineChangeDTO headlineChange = new()
            {
                Id = 1,
                ArticleId = 1,
                Detected = new DateTime(2020, 10, 20),
                TitleBefore = "NEWtitleBefore",
                TitleAfter = "NEWtitleAfter",
                UpvoteCount = 999
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _headlineChangeDaoMock.Setup(x => x.AssertExistsAsync(headlineChange.Id))
                .ReturnsAsync(_data.HeadlineChange1);

            //Act
            HeadlineChangeDTO result = await _sut.CreateOrUpdateHeadlineChangeAsync(headlineChange);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _headlineChangeDaoMock.Verify(x => x.AssertExistsAsync(headlineChange.Id), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(headlineChange.Id);
            result.ArticleId.Should().Be(headlineChange.ArticleId);
            result.Detected.Should().Be(headlineChange.Detected);
            result.TitleBefore.Should().Be(headlineChange.TitleBefore);
            result.TitleAfter.Should().Be(headlineChange.TitleAfter);
            result.UpvoteCount.Should().Be(headlineChange.UpvoteCount);
        }

        [Fact]
        public async Task GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetOrderByUpvotesCountIncludeArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.HeadlineChanges);

            //Act
            List<HeadlineChangeDTO> result = await _sut.GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync();

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);

            result.Should().NotBeNull();
            result.Should().HaveCount(_data.HeadlineChanges.Count);
        }

        [Fact]
        public async Task GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetOrderByDetectedDescendingIncludeArticleAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.HeadlineChanges);

            //Act
            List<HeadlineChangeDTO> result = await _sut.GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync(10, 10);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);

            result.Should().NotBeNull();
            result.Should().HaveCount(_data.HeadlineChanges.Count);
        }

        [Fact]
        public async Task GetHeadlineChangeCountAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetCountAsync(It.IsAny<long?>()))
                .ReturnsAsync(_data.HeadlineChanges.Count);

            //Act
            long result = await _sut.GetHeadlineChangeCountAsync();

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);

            result.Should().Be(_data.HeadlineChanges.Count);
        }

        [Fact]
        public async Task GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetByArticleIdOrderByDetectedDescendingAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.HeadlineChanges);

            //Act
            List<HeadlineChangeDTO> result = await _sut.GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(1, 10, 10);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);

            result.Should().NotBeNull();
            result.Should().HaveCount(_data.HeadlineChanges.Count);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(-5)]
        public async Task AddUpvotesToHeadlineChangeAsync_Simple(int amount)
        {
            //Arrange
            long originalUpvoteCount = 0;

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _headlineChangeDaoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new HeadlineChange() { UpvoteCount = originalUpvoteCount });

            //Act
            HeadlineChangeDTO result = await _sut.AddUpvotesToHeadlineChangeAsync(1, amount);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);

            result.UpvoteCount.Should().Be(originalUpvoteCount + amount);
        }

        [Fact]
        public async Task AddUpvotesToHeadlineChangeAsync_WhenHeadlineChangeDoesNotExist_ThrowsException()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetByIdAsync(1))
                .Returns(Task.FromResult<HeadlineChange>(null));

            //Act
            Func<Task> act = async () => await _sut.AddUpvotesToHeadlineChangeAsync(1, 1);

            //Assert
            await act.Should().ThrowAsync<Exception>().WithMessage($"HeadlineChange with Id '{1}' not found.");

            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteHeadlineChangeAsync_ShouldDeleteHeadlineChange()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _headlineChangeDaoMock.Setup(x => x.GetByIdAsync(_data.HeadlineChange1.Id))
                .ReturnsAsync(_data.HeadlineChange1);
            _headlineChangeDaoMock.Setup(x => x.Delete(_data.HeadlineChange1.Id));

            //Act
            HeadlineChangeDTO result = await _sut.DeleteHeadlineChangeAsync(new HeadlineChangeDTO { Id = _data.HeadlineChange1.Id });

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);
            _headlineChangeDaoMock.Verify(x => x.Delete(_data.HeadlineChange1.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteHeadlineChangeAsync_WhenHeadlineChangeDoesNotExist_ShouldThrowException()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());
            _headlineChangeDaoMock.Setup(x => x.GetByIdAsync(_data.HeadlineChange1.Id))
                .Returns(Task.FromResult<HeadlineChange>(null));

            //Act
            Func<Task> act = async () => await _sut.DeleteHeadlineChangeAsync(new HeadlineChangeDTO { Id = _data.HeadlineChange1.Id });

            //Assert
            await act.Should().ThrowAsync<Exception>().WithMessage($"HeadlineChange with Id '{_data.HeadlineChange1.Id}' does not exist.");

            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
            _headlineChangeDaoMock.Verify(x => x.Delete(It.IsAny<long>()), Times.Never);
        }

        private sealed class TestData
        {
            public List<HeadlineChange> HeadlineChanges => new() { HeadlineChange1, HeadlineChange2 };

            public HeadlineChange HeadlineChange1 => new()
            {
                Id = 1,
                ArticleId = 1,
                Detected = new DateTime(2020, 10, 10),
                TitleBefore = "titleBefore1",
                TitleAfter = "titleAfter1",
                UpvoteCount = 1
            };

            public HeadlineChange HeadlineChange2 => new()
            {
                Id = 2,
                ArticleId = 2,
                Detected = new DateTime(2020, 10, 11),
                TitleBefore = "titleBefore2",
                TitleAfter = "titleAfter2",
                UpvoteCount = 0
            };
        }
    }
}