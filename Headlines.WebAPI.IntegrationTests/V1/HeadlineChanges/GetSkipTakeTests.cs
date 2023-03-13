using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Controllers.V1;
using Headlines.WebAPI.Resources.V1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.HeadlineChanges
{
    public sealed class GetSkipTakeTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetSkipTakeTests(WebAPIFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Theory]
        [InlineData(120, 0)]
        [InlineData(120, 1)]
        [InlineData(120, 10)]
        [InlineData(120, 100)]
        [InlineData(10, 20)]
        [InlineData(10, 100)]
        public async Task GetSkipTake_WhenSkipIsZero_ShouldReturnCorrectCount(int count, int take)
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(count));

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/Skip/{0}/Take/{take}");
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.HeadlineChanges.Should().NotBeNull();
            content.HeadlineChanges.Count.Should().Be(Math.Min(count, Math.Min(take, HeadlineChangesController.MaxTake)));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 10, 10)]
        [InlineData(10, 5, 10)]
        [InlineData(10, 10, 10)]
        [InlineData(100, 10, 150)]
        public async Task GetSkipTake_WhenSkipIsLargerThanZero_ShouldReturnCorrectCount(int count, int skip, int take)
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(count));

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/Skip/{skip}/Take/{take}");
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.HeadlineChanges.Should().NotBeNull();
            content.HeadlineChanges.Count.Should().Be(Math.Min(Math.Max(count - skip, 0), Math.Min(take, HeadlineChangesController.MaxTake)));
        }

        [Fact]
        public async Task GetSkipTake_ShouldReturnCorrectOrder()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(100));

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/Skip/0/Take/{HeadlineChangesController.MaxTake}");
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.HeadlineChanges.Should().NotBeNull();
            content.HeadlineChanges.Should().BeInDescendingOrder(x => x.Detected);
        }

        [Fact]
        public async Task GetSkipTake_Simple_ShouldReturnCorrectMapping()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(HeadlineChangesController.DefaultTake));

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/Skip/0/Take/{HeadlineChangesController.DefaultTake}");
            var content = await response.Content.ReadAsAsync<GetTopUpvotedResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.HeadlineChanges.Should().NotBeNull();

            List<HeadlineChangeDto> dataOrdered = data.OrderByDescending(x => x.Detected).ToList();
            for (int i = 0; i < dataOrdered.Count; i++)
            {
                var actual = content.HeadlineChanges[i];
                var expected = dataOrdered[i];

                actual.Id.Should().Be(expected.Id);
                actual.ArticleId.Should().Be(expected.ArticleId);
                actual.Detected.Should().Be(expected.Detected);
                actual.TitleBefore.Should().Be(expected.TitleBefore);
                actual.TitleAfter.Should().Be(expected.TitleAfter);
                actual.UpvoteCount.Should().Be(expected.UpvoteCount);

                actual.Article.Should().NotBeNull();
                actual.Article?.Id.Should().Be(expected.Article.Id);
                actual.Article?.Published.Should().Be(expected.Article.Published);
                actual.Article?.UrlId.Should().Be(expected.Article.UrlId);
                actual.Article?.CurrentTitle.Should().Be(expected.Article.CurrentTitle);
                actual.Article?.Link.Should().Be(expected.Article.Link);
            }
        }

        [Fact]
        public async Task GetSkipTake_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(HeadlineChangesController.DefaultTake));

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/Skip/0/Take/{HeadlineChangesController.DefaultTake}");
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetSkipTakeResponse>();
        }
    }
}