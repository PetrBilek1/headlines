using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.HeadlineChanges
{
    public sealed class GetByArticleIdSkipTakeTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetByArticleIdSkipTakeTests(WebAPIFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Theory]
        [InlineData(0, 0, 10, 5)]
        [InlineData(10, 10, 10, 5)]
        [InlineData(5, 10, 10, 3)]
        [InlineData(10, 150, 12, 6)]
        public async Task GetByArticleIdSkipTake_ShouldReturnHeadlineChanges(int skip, int take, int count, int matchingCount)
        {
            //Arrange
            List<HeadlineChangeDTO> data = DataGenerator.GenerateHeadlineChanges(count).ToList();
            ArticleDTO matchingArticle = data.First().Article;

            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            matchingArticle = (await populator.InsertArticlesAsync(new List<ArticleDTO> { matchingArticle })).First();

            for (int i = 0; i < matchingCount; i++) 
            {
                data[i].Article = matchingArticle;
            }

            data = await populator.InsertHeadlineChangesAsync(data);
            data = data.OrderByDescending(x => x.Detected).ToList();

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/GetByArticleIdSkipTake?articleId={matchingArticle.Id}&skip={skip}&take={take}");
            var content = await response.Content.ReadAsAsync<GetByArticleIdSkipTakeResponse>();

            //Assert
            var expectedData = data.Where(x => x.ArticleId == matchingArticle.Id).ToList();
            expectedData = skip >= expectedData.Count
                ? new List<HeadlineChangeDTO>()
                : expectedData.GetRange(skip, Math.Max(0, Math.Min(expectedData.Count - skip, take)));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.HeadlineChanges.Should().NotBeNull();
            content.HeadlineChanges.Count.Should().Be(expectedData.Count);
            content.TotalCount.Should().Be(matchingCount);

            for (int i = 0; i < expectedData.Count; i++)
            {
                var expected = expectedData[i];
                var actual = content.HeadlineChanges[i];

                actual.Id.Should().Be(expected.Id);
                actual.ArticleId.Should().Be(expected.ArticleId);
                actual.Detected.Should().Be(expected.Detected);
                actual.TitleBefore.Should().Be(expected.TitleBefore);
                actual.TitleAfter.Should().Be(expected.TitleAfter);
                actual.UpvoteCount.Should().Be(expected.UpvoteCount);
            }
        }

        [Fact]
        public async Task GetByArticleIdSkipTake_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.GetAsync($"/v1/HeadlineChanges/GetByArticleIdSkipTake?articleId={headlineChange.ArticleId}&skip={0}&take={0}");
            var content = await response.Content.ReadAsAsync<GetByArticleIdSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetByArticleIdSkipTakeResponse>();
        }
    }
}