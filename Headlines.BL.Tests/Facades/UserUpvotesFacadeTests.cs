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
    public sealed class UserUpvotesFacadeTests
    {
        private readonly UserUpvotesFacade _sut;
        private readonly TestData _data;

        private readonly Mock<IUnitOfWorkProvider> _uowProviderMock = new Mock<IUnitOfWorkProvider>(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly Mock<IUserUpvotesDao> _userUpvotesDaoMock = new Mock<IUserUpvotesDao>(MockBehavior.Strict);
        private readonly IMapper _mapper = TestUtils.GetMapper();

        public UserUpvotesFacadeTests()
        {
            _sut = new UserUpvotesFacade(_uowProviderMock.Object, _userUpvotesDaoMock.Object, _mapper);
            _data = new TestData();
        }

        [Fact]
        public async Task CreateOrUpdateUserUpvotesAsync_ShouldCreateUserUpvotes()
        {
            //Arrange
            UserUpvotesDto upvotes = new()
            {
                Id = default,
                UserToken = "userToken",
                Json = "json"
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _userUpvotesDaoMock.Setup(x => x.InsertAsync(It.IsAny<UserUpvotes>()))
                .ReturnsAsync(new UserUpvotes() { Id = 1 });

            //Act
            UserUpvotesDto result = await _sut.CreateOrUpdateUserUpvotesAsync(upvotes);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once());
            _uowMock.Verify(x => x.CommitAsync(), Times.Once());
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _userUpvotesDaoMock.Verify(x => x.InsertAsync(It.IsAny<UserUpvotes>()), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.UserToken.Should().Be(upvotes.UserToken);
            result.Json.Should().Be(upvotes.Json);
        }

        [Fact]
        public async Task CreateOrUpdateUserUpvotesAsync_ShouldUpdateUserUpvotes()
        {
            //Arrange
            UserUpvotesDto upvotes = new()
            {
                Id = 1,
                UserToken = "NEWuserToken",
                Json = "NEWjson"
            };

            _uowProviderMock.Setup(x => x.CreateUnitOfWork())
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.Dispose());

            _userUpvotesDaoMock.Setup(x => x.AssertExistsAsync(_data.UserUpvotes1.Id))
                .ReturnsAsync(_data.UserUpvotes1);

            //Act
            UserUpvotesDto result = await _sut.CreateOrUpdateUserUpvotesAsync(upvotes);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);
            _uowMock.Verify(x => x.Dispose(), Times.Once);
            _userUpvotesDaoMock.Verify(x => x.AssertExistsAsync(_data.UserUpvotes1.Id), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.UserToken.Should().Be(upvotes.UserToken);
            result.Json.Should().Be(upvotes.Json);
        }

        [Fact]
        public async Task GetUserUpvotesByUserTokenAsync_Simple()
        {
            //Arrange
            _uowProviderMock.Setup(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking))
                .Returns(_uowMock.Object);
            _uowMock.Setup(x => x.Dispose());

            _userUpvotesDaoMock.Setup(x => x.GetByUserTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_data.UserUpvotes1);

            //Act
            UserUpvotesDto result = await _sut.GetUserUpvotesByUserTokenAsync(_data.UserUpvotes1.UserToken);

            //Assert
            _uowProviderMock.Verify(x => x.CreateUnitOfWork(EntityTrackingOptions.NoTracking), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
            _uowMock.Verify(x => x.Dispose(), Times.Once);

            result.Should().NotBeNull();
            result.Id.Should().Be(_data.UserUpvotes1.Id);
            result.UserToken.Should().Be(_data.UserUpvotes1.UserToken);
            result.Json.Should().Be(_data.UserUpvotes1.Json);
        }

        private sealed class TestData
        {
            public UserUpvotes UserUpvotes1 = new()
            {
                Id = 1,
                UserToken = "userToken",
                Json = "json"
            };
        }
    }
}