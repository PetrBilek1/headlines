using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Requests.HeadlineChanges;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Controllers.v1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Articles
{
    [Collection(SerialLine.One)]
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

        [Theory]
        [InlineData(0, 5, 10, 1)]
        [InlineData(0, 10, 20, 3)]
        [InlineData(5, 10, 20, 5)]
        [InlineData(5, 10, 10, 2)]
        [InlineData(5, 1000, ArticlesController.MaxTake + 10, 8)]
        public async Task GetSkipTake_WhenSearchPromptFilter_ShouldReturnMatchingArticlesAndCount(int skip, int take, int count, int promptMatchCount)
        {
            //Arrange
            string prompt = "andrej";
            Random random = new Random(66666);

            List<ArticleDTO> articles = DataGenerator.GenerateArticles(count).ToList();
            List<ArticleDTO> matchingArticles = new();
            for (int i = 0; i < promptMatchCount; i++)
            {
                string[] words = articles[i].CurrentTitle.Split(' ');
                words[random.Next(0, words.Count())] = prompt;
                articles[i].CurrentTitle = string.Join(" ", words);
                matchingArticles.Add(articles[i]);
            }
            matchingArticles = matchingArticles.OrderByDescending(x => x.Published).ToList();

            await using var populator = DatabasePopulator.Create(_serviceProvider);
            articles = await populator.InsertArticlesAsync(articles);
            for (int i = 0; i < articles.Count(); i++)
            {
                var articleMatch = matchingArticles.FirstOrDefault(x => x.CurrentTitle == articles[i].CurrentTitle);
                if (articleMatch == null)
                    continue;

                articleMatch.Id = articles[i].Id;
                articleMatch.SourceId = articles[i].SourceId;
            }

            //gives fts time to index
            Thread.Sleep(7500);

            //Act
            var response = await _client.PostAsJsonAsync("/v1/Articles/GetSkipTake", new GetSkipTakeRequest()
            {
                Skip = skip,
                Take = take,
                SearchPrompt = prompt,
                ArticleSources = null
            });
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();

            content.Articles.Should().NotBeNull();
            content.Articles.Should().HaveCount(Math.Max(0, Math.Min(ArticlesController.MaxTake, Math.Min(promptMatchCount - skip, take))));
            content.MatchesFiltersCount.Should().Be(promptMatchCount);

            var expectedMatchingArticles = skip >= matchingArticles.Count
                ? new List<ArticleDTO>()
                : matchingArticles.GetRange(skip, Math.Max(0, Math.Min(matchingArticles.Count() - skip, take)));

            for (int i = 0; i < expectedMatchingArticles.Count(); i++)
            {
                AssertArticle(content.Articles[i], expectedMatchingArticles[i]);
            }
        }

        [Theory]
        [InlineData(0, 5, 10)]
        [InlineData(0, 10, 20)]
        [InlineData(5, 10, 20)]
        [InlineData(5, 10, 10)]
        [InlineData(5, 1000, ArticlesController.MaxTake + 10)]
        public async Task GetSkipTake_WhenArticleSourceFilter_ShouldReturnMatchingArticlesAndCount(int skip, int take, int count)
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(count));
            var articleSourceIds = data.Select(x => x.SourceId).ToList();

            var articleSourcesFilter = articleSourceIds.Where(x => x % 2 == 0).ToArray();
            var articlesMatchingFilter = data.Where(x => articleSourcesFilter.Contains(x.SourceId)).OrderByDescending(x => x.Published).ToList();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/Articles/GetSkipTake", new GetSkipTakeRequest()
            {
                Skip = skip,
                Take = take,
                SearchPrompt = null,
                ArticleSources = articleSourcesFilter
            });
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();

            content.Articles.Should().NotBeNull();
            content.Articles.Should().HaveCount(Math.Max(0, Math.Min(ArticlesController.MaxTake, Math.Min(articlesMatchingFilter.Count - skip, take))));
            content.MatchesFiltersCount.Should().Be(articlesMatchingFilter.Count);

            var expectedMatchingArticles = skip >= articlesMatchingFilter.Count
                ? new List<ArticleDTO>()
                : articlesMatchingFilter.GetRange(skip, Math.Max(0, Math.Min(articlesMatchingFilter.Count - skip, take)));

            for (int i = 0; i < expectedMatchingArticles.Count; i++)
            {
                AssertArticle(content.Articles[i], expectedMatchingArticles[i]);
            }
        }

        [Fact]
        public async Task GetSkipTake_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var userToken = Guid.NewGuid().ToString();
            var headlineChange = (await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(1))).First();

            //Act
            var response = await _client.PostAsJsonAsync("/v1/Articles/GetSkipTake", new GetSkipTakeRequest()
            {
                Skip = 0,
                Take = 10,
                SearchPrompt = null,
                ArticleSources = null
            });
            var content = await response.Content.ReadAsAsync<GetSkipTakeResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetSkipTakeResponse>();
        }

        private void AssertArticle(ArticleModel actual, ArticleDTO expected)
        {
            actual.Id.Should().Be(expected.Id);
            actual.SourceId.Should().Be(expected.SourceId);
            actual.Published.Should().Be(expected.Published);
            actual.UrlId.Should().Be(expected.UrlId);
            actual.CurrentTitle.Should().Be(expected.CurrentTitle);
            actual.Link.Should().Be(expected.Link);
        }
    }
}