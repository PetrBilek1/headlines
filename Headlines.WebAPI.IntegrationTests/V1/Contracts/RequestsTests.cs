using FluentAssertions;
using Headlines.WebAPI.Contracts;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Requests.HeadlineChanges;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Contracts
{
    public sealed class RequestsTests
    {
        private readonly string _requiredAttribute = "System.ComponentModel.DataAnnotations.RequiredAttribute";
        private readonly string _jsonPropertyAttribute = "Newtonsoft.Json.JsonPropertyAttribute";

        [Fact]
        public void ShouldCoverAllRequestsByTests()
        {
            //Arrange            
            var tests = typeof(RequestsTests)
                .GetMethods()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType.FullName == "Xunit.FactAttribute"))
                .Select(x => x.Name)
                .ToHashSet();

            var requests = typeof(IApiContractsMarker).Assembly
                .GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("Headlines.WebAPI.Contracts.V1.Requests"))
                .Select(x => $"{x.Namespace!.Replace("Headlines.WebAPI.Contracts.V1.Requests.", "")}_{x.Name}")
                .ToList();

            //Assert
            requests.Should().HaveCountGreaterThan(0);

            foreach (var request in requests)
            {
                tests.Should().Contain(request);
            }
        }

        [Fact]
        public void Articles_GetSkipTakeRequest()
        {
            //Assert
            TestExtensions.AssertProperties<GetSkipTakeRequest>(new()
            {
                ("Skip", typeof(int?), new string[] { _jsonPropertyAttribute }),
                ("Take", typeof(int?), new string[] { _jsonPropertyAttribute }),
                ("SearchPrompt", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("ArticleSources", typeof(long[]), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void Articles_RequestDetailScrapeRequest()
        {
            //Assert
            TestExtensions.AssertProperties<RequestDetailScrapeRequest>(new()
            {
                ("ArticleId", typeof(long), new string[] { _jsonPropertyAttribute, _requiredAttribute }),
            });
        }

        [Fact]
        public void HeadlineChanges_UpvoteRequest()
        {
            //Assert
            TestExtensions.AssertProperties<UpvoteRequest>(new()
            {
                ("HeadlineChangeId", typeof(long), new string[] { _requiredAttribute, _jsonPropertyAttribute }),
                ("UserToken", typeof(string), new string[] { _requiredAttribute, _jsonPropertyAttribute })
            });
        }
    }
}