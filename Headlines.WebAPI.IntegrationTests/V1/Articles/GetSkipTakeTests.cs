using FluentAssertions;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Controllers.v1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Articles
{
    [Collection("Articles")]
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
        [InlineData(0, 5, 10)]
        [InlineData(0, 10, 20)]
        [InlineData(5, 10, 20)]
        [InlineData(5, 10, 10)]
        [InlineData(5, 1000, ArticlesController.MaxTake + 10)]
        public async Task GetSkipTake_WhenNoFilters_ShouldReturnArticlesAndCount(int skip, int take, int count)
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(count));

            //Act
            var response = await _client.PostAsJsonAsync("/v1/Articles/GetSkipTake", new GetSkipTakeRequest()
            {
                Skip = skip,
                Take = take,
                SearchPrompt = null,
                ArticleSources = null
            });
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();

            content.Articles.Should().NotBeNull();
            content.Articles.Should().HaveCount(Math.Max(0, Math.Min(ArticlesController.MaxTake, Math.Min(count - skip, take))));
            content.MatchesFiltersCount.Should().Be(count);
        }
    }
}