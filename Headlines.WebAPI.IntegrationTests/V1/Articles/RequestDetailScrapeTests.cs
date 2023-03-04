using FluentAssertions;
using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Events;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Resources.V1;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using Moq;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Articles
{
    public sealed class RequestDetailScrapeTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        private readonly Mock<IEventBus> _eventBusMock;

        public RequestDetailScrapeTests(WebAPIFactory apiFactory)
        {
            apiFactory.MockObjectStorageWrapper();
            _eventBusMock = apiFactory.MockEventBus();

            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Fact]
        public async Task RequestDetailScrape_WhenArticleDoesNotExist_ShouldReturnNotFound()
        {
            //Act
            var response = await _client.PostAsJsonAsync($"/v1/Articles/RequestDetailScrape", new RequestDetailScrapeRequest
            {
                ArticleId = 1
            });
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Be(Messages.M0004);
        }

        [Fact]
        public async Task RequestDetailScrape_WhenArticleSourceScraperTypeNull_ShouldReturnBadRequest()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(1));
            var article = data.First();

            //Act
            var response = await _client.PostAsJsonAsync($"/v1/Articles/RequestDetailScrape", new RequestDetailScrapeRequest
            {
                ArticleId = article.Id
            });
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Be(Messages.M0005);
        }

        [Fact]
        public async Task RequestDetailScrape_ShouldPublishScrapeRequestToEventBus()
        {
            //Arrange
            await using var populator = await DatabasePopulator.CreateAsync(_serviceProvider);
            var data = await populator.InsertArticlesAsync(DataGenerator.GenerateArticles(1, true));
            var article = data.First();

            _eventBusMock.Setup(x => x.PublishAsync(It.IsAny<ArticleDetailScrapeRequestedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //Act
            var response = await _client.PostAsJsonAsync($"/v1/Articles/RequestDetailScrape", new RequestDetailScrapeRequest
            {
                ArticleId = article.Id
            });
            var content = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ArticleDetailScrapeRequestedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}