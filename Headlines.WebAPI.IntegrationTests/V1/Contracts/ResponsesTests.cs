using FluentAssertions;
using Headlines.WebAPI.Contracts;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Responses.ArticleSources;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Contracts.V1.Responses.UserUpvotes;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Contracts
{
    public sealed class ResponsesTests
    {
        private readonly string _jsonPropertyAttribute = "Newtonsoft.Json.JsonPropertyAttribute";

        [Fact]
        public void ShouldCoverAllResponsesByTests()
        {
            //Arrange
            var tests = typeof(ResponsesTests)
                .GetMethods()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType.FullName == "Xunit.FactAttribute"))
                .Select(x => x.Name)
                .ToHashSet();

            var responses = typeof(IApiContractsMarker).Assembly
                .GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("Headlines.WebAPI.Contracts.V1.Responses"))
                .Select(x => $"{x.Namespace!.Replace("Headlines.WebAPI.Contracts.V1.Responses.", "")}_{x.Name}")
                .ToList();

            //Assert
            responses.Should().HaveCountGreaterThan(0);

            foreach (var response in responses)
            {
                tests.Should().Contain(response);
            }
        }

        [Fact]
        public void ArticleSources_GetAllResponse()
        {
            //Assert
            TestExtensions.AssertProperties<GetAllResponse>(new()
            {
                ("ArticleSources", typeof(List<ArticleSourceModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_GetSkipTakeResponse()
        {
            //Assert
            TestExtensions.AssertProperties<GetSkipTakeResponse>(new()
            {
                ("HeadlineChanges", typeof(List<HeadlineChangeModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_GetTopUpvotedResponse()
        {
            //Assert
            TestExtensions.AssertProperties<GetTopUpvotedResponse>(new()
            {
                ("HeadlineChanges", typeof(List<HeadlineChangeModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_UpvoteResponse()
        {
            //Assert
            TestExtensions.AssertProperties<UpvoteResponse>(new()
            {
                ("Upvotes", typeof(List<UpvoteModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void UserUpvotes_GetResponse()
        {
            //Assert
            TestExtensions.AssertProperties<GetResponse>(new()
            {
                ("Upvotes", typeof(List<UpvoteModel>), new string[] { _jsonPropertyAttribute })
            });
        }
    }
}