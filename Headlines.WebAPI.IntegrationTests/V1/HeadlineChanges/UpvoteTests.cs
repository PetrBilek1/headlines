using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Requests.HeadlineChanges;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.HeadlineChanges
{
    [Collection("HeadlineChanges")]
    public sealed class UpvoteTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly DateTime _now = new DateTime(2022, 10, 10);

        public UpvoteTests(WebAPIFactory apiFactory)
        {
            apiFactory.MockDateTimeProvider(_now);

            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task Upvote_WhenNotUpvoted_ShouldUpvote()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/HeadlineChanges/Upvote", new UpvoteRequest()
            {
                HeadlineChangeId = headlineChange.Id,
                UserToken = userToken
            });
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);
        }

        [Fact]
        public async Task Upvote_WhenAlreadyUpvoted_ShouldNotUpvote()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();
            var originalUpvoteDate = new DateTime(2020, 10, 10);
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
                        Date = originalUpvoteDate
                    }
                })
            });

            //Act
            var response = await _client.PostAsJsonAsync("/v1/HeadlineChanges/Upvote", new UpvoteRequest()
            {
                HeadlineChangeId = headlineChange.Id,
                UserToken = userToken
            });
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);
            content.Upvotes.First().Date.Should().Be(originalUpvoteDate);
        }

        [Fact]
        public async Task Upvote_WhenNotUpvoted_ShouldEditDatabase()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/HeadlineChanges/Upvote", new UpvoteRequest()
            {
                HeadlineChangeId = headlineChange.Id,
                UserToken = userToken
            });
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);

            var databaseUserUpvotes = await populator.GetAllUserUpvotesAsync();
            var databaseHeadlineChanges = await populator.GetAllHeadlineChangesAsync();

            databaseUserUpvotes.Should().HaveCount(1);
            databaseUserUpvotes.Should().Contain(x => x.UserToken == userToken);

            var upvotes = TestExtensions.JsonToUpvoteList(databaseUserUpvotes.First(x => x.UserToken == userToken).Json);
            upvotes.Should().NotBeNull();
            upvotes.Should().HaveCount(1);
            upvotes.First().Type.Should().Be(UpvoteType.HeadlineChange);
            upvotes.First().TargetId.Should().Be(headlineChange.Id);
            upvotes.First().Date.Should().Be(_now);

            databaseHeadlineChanges.Should().HaveCount(1);
            databaseHeadlineChanges.First().Id.Should().Be(headlineChange.Id);
            databaseHeadlineChanges.First().UpvoteCount.Should().Be(headlineChange.UpvoteCount + 1);
        }

        [Fact]
        public async Task Upvote_ShouldReturnCorrectMapping()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/HeadlineChanges/Upvote", new UpvoteRequest()
            {
                HeadlineChangeId = headlineChange.Id,
                UserToken = userToken
            });
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Upvotes.Should().NotBeNull();
            content.Upvotes.Should().HaveCount(1);

            content.Upvotes.First().Type.Should().Be(UpvoteType.HeadlineChange);
            content.Upvotes.First().TargetId.Should().Be(headlineChange.Id);
            content.Upvotes.First().Date.Should().Be(_now);
        }

        [Fact]
        public async Task Upvote_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/HeadlineChanges/Upvote", new UpvoteRequest()
            {
                HeadlineChangeId = headlineChange.Id,
                UserToken = userToken
            });
            var content = await response.Content.ReadAsAsync<UpvoteResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<UpvoteResponse>();
        }
    }
}