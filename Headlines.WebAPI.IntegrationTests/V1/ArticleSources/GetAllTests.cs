using FluentAssertions;
using Headlines.WebAPI.Contracts.V1.Responses.ArticleSources;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.ArticleSources
{
    public class GetAllTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetAllTests(WebAPIFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task GetAll_ShouldReturnArticleSources()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            await populator.InsertArticleSourcesAsync(DataGenerator.GenerateArticleSources(10));

            //Act
            var response = await _client.GetAsync($"/v1/ArticleSources/GetAll");
            var content = await response.Content.ReadAsAsync<GetAllResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.ArticleSources.Should().NotBeNull();
            content.ArticleSources.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCorrectMapping()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var data = await populator.InsertArticleSourcesAsync(DataGenerator.GenerateArticleSources(10));

            //Act
            var response = await _client.GetAsync($"/v1/ArticleSources/GetAll");
            var content = await response.Content.ReadAsAsync<GetAllResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            
            for (int i = 0; i < data.Count; i++)
            {
                var expected = data[i];
                var actual = content.ArticleSources[i];

                actual.Id.Should().Be(expected.Id);
                actual.Name.Should().Be(expected.Name);
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            var data = await populator.InsertArticleSourcesAsync(DataGenerator.GenerateArticleSources(10));

            //Act
            var response = await _client.GetAsync($"/v1/ArticleSources/GetAll");
            var content = await response.Content.ReadAsAsync<GetAllResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetAllResponse>();
        }
    }
}