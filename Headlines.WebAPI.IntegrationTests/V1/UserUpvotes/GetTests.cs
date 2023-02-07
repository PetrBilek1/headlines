using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Contracts.V1.Responses.UserUpvotes;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.UserUpvotes
{
    public sealed class GetTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetTests(WebAPIFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task Get_WhenUserTokenSpecified_ShouldReturnUserUpvotes()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();
            var userUpvotes = await populator.InsertUserUpvotesAsync(new UserUpvotesDTO()
            {
                Id = default,
                UserToken = userToken,
                Json = TestExtensions.UpvoteListToJson(new List<UpvoteModel>()
                {
                    new UpvoteModel()
                    {
                        Type = UpvoteType.HeadlineChange,
                        TargetId = headlineChange.Id,
                        Date = new DateTime(2020, 10, 10)
                    }
                })
            });

            //Act
            var response = await _client.GetAsync($"/v1/UserUpvotes/Get?userToken={userToken}");
            var content = await response.Content.ReadAsAsync<GetResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_WhenUserTokenNotSpecified_ShouldReturnBadRequest()
        {
            //Act
            var response = await _client.GetAsync($"/v1/UserUpvotes/Get?userToken=");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_ShouldReturnCorrectMapping()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);

            var userToken = Guid.NewGuid().ToString();
            var targetId = 1;
            var originalDate = new DateTime(2020, 10, 10);

            var userUpvotes = await populator.InsertUserUpvotesAsync(new UserUpvotesDTO()
            {
                Id = default,
                UserToken = userToken,
                Json = TestExtensions.UpvoteListToJson(new List<UpvoteModel>()
                {
                    new UpvoteModel()
                    {
                        Type = UpvoteType.HeadlineChange,
                        TargetId = targetId,
                        Date = originalDate
                    }
                })
            });

            //Act
            var response = await _client.GetAsync($"/v1/UserUpvotes/Get?userToken={userToken}");
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);

            content.Upvotes.First().Type.Should().Be(UpvoteType.HeadlineChange);
            content.Upvotes.First().TargetId.Should().Be(targetId);
            content.Upvotes.First().Date.Should().Be(originalDate);
        }

        [Fact]
        public async Task Get_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();
            var userUpvotes = await populator.InsertUserUpvotesAsync(new UserUpvotesDTO()
            {
                Id = default,
                UserToken = userToken,
                Json = TestExtensions.UpvoteListToJson(new List<UpvoteModel>()
                {
                    new UpvoteModel()
                    {
                        Type = UpvoteType.HeadlineChange,
                        TargetId = headlineChange.Id,
                        Date = new DateTime(2020, 10, 10)
                    }
                })
            });

            //Act
            var response = await _client.GetAsync($"/v1/UserUpvotes/Get?userToken={userToken}");
            var content = await response.Content.ReadAsAsync<GetResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetResponse>();
        }
    }
}