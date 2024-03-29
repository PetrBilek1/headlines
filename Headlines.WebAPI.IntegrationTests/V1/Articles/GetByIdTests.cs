﻿using FluentAssertions;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Resources.V1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Articles
{
    public sealed class GetByIdTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetByIdTests(WebAPIFactory apiFactory)
        {
            apiFactory.MockObjectStorageWrapper();

            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task GetById_WhenArticleDoesNotExist_ShouldReturnNotFound()
        {
            //Act
            var response = await _client.GetAsync($"/v1/Articles/999999999");
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetById_Simple()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(2));

            //Act
            var response = await _client.GetAsync($"/v1/Articles/{data.First().Id}");
            var content = await response.Content.ReadAsAsync<GetByIdResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Article.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectContract()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(1));

            //Act
            var response = await _client.GetAsync($"/v1/Articles/{data.First().Id}");
            var content = await response.Content.ReadAsAsync<GetByIdResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().BeOfType<GetByIdResponse>();
        }
    }
}