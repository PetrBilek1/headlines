using FluentAssertions;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Net;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.HeadlineChanges
{
    [Collection("HeadlineChanges")]
    public sealed class GetCountTests : IClassFixture<WebAPIFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GetCountTests(WebAPIFactory apiFactory)
        {
            _client = apiFactory.CreateClient();
            _serviceProvider = apiFactory.Services;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(120)]
        public async Task GetCount_ShouldReturnCount(int count)
        {
            //Arrange
            await using var populator = DatabasePopulator.Create(_serviceProvider);
            await populator.InsertHeadlineChangesAsync(DataGenerator.GenerateHeadlineChanges(count));

            //Act
            var response = await _client.GetAsync("/v1/HeadlineChanges/GetCount");
            var content = await response.Content.ReadAsAsync<long>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be(count);
        }
    }
}