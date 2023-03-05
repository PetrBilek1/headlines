using FluentAssertions;
using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Resources.V1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Articles
{
    public sealed class GetDetailByIdTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetDetailByIdTests(WebAPIFactory apiFactory)
        {
            apiFactory.MockObjectStorageWrapper();

            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task GetDetailById_WhenArticleIdNotSet_ShouldReturnBadRequest()
        {
            //Act
            var response = await _client.GetAsync($"/v1/Articles/GetDetailById");
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Be(Messages.M0003);
        }

        [Fact]
        public async Task GetDetailById_WhenArticleDoesNotExist_ShouldReturnNotFound()
        {
            //Act
            var response = await _client.GetAsync($"/v1/Articles/GetDetailById?id=1");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDetailById_WhenArticleHasNoDetails_ShouldReturnNullDetail()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var article = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(1));

            //Act
            var response = await _client.GetAsync($"/v1/Articles/GetDetailById?id={article.First().Id}");
            var content = await response.Content.ReadAsAsync<GetDetailByIdResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Detail.Should().BeNull();
        }

        [Theory]
        [InlineData(5)]
        public async Task GetDetailById_WhenArticleHasDetails_ShouldReturnLatestCreatedDetail(int articleDetailCount)
        {
            //Arrange
            var article = DataGenerator.ArticleDTO(articleDetailCount)
                .RuleFor(x => x.Source, DataGenerator.ArticleSourceDTO())
                .Generate();

            var articleDetails = DataGenerator.GenerateArticleDetails(articleDetailCount).ToList();
            var objectStorage = _serviceProvider.GetRequiredService<IObjectStorageWrapper>();

            var detailsByKeys = new Dictionary<string, ArticleDetailDTO>();
            for(int i = 0; i < articleDetailCount; i++)
            {
                var objectData = await objectStorage.UploadObjectAsync(articleDetails[i], article.Details[i].Bucket);
                article.Details[i].Key = objectData.Key;

                detailsByKeys.Add(objectData.Key, articleDetails[i]);
            }

            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            article = (await populator.InsertArticlesAsync(new List<ArticleDTO> { article })).First();

            var expectedDetail = detailsByKeys[article.Details.OrderByDescending(x => x.Created).First().Key];

            //Act
            var response = await _client.GetAsync($"/v1/Articles/GetDetailById?id={article.Id}");
            var content = await response.Content.ReadAsAsync<GetDetailByIdResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Detail.Should().NotBeNull();
            content.Detail.IsPaywalled.Should().Be(expectedDetail.IsPaywalled);
            content.Detail.Title.Should().Be(expectedDetail.Title);
            content.Detail.Author.Should().Be(expectedDetail.Author);

            content.Detail.Paragraphs.Should().HaveCount(expectedDetail.Paragraphs.Count);
            for(int i = 0; i < expectedDetail.Paragraphs.Count; i++)
            {
                var actual = content.Detail.Paragraphs[i];
                var expected = expectedDetail.Paragraphs[i];

                actual.Should().Be(expected);
            }

            content.Detail.Tags.Should().HaveCount(expectedDetail.Tags.Count);
            for (int i = 0; i < expectedDetail.Tags.Count; i++)
            {
                var actual = content.Detail.Tags[i];
                var expected = expectedDetail.Tags[i];

                actual.Should().Be(expected);
            }
        }

        [Fact]
        public async Task GetDetailById_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(1));

            //Act
            var response = await _client.GetAsync($"/v1/Articles/GetDetailById?id={data.First().Id}");
            var content = await response.Content.ReadAsAsync<GetDetailByIdResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetDetailByIdResponse>();
        }
    }
}