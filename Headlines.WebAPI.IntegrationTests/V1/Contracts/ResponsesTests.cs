using FluentAssertions;
using Headlines.WebAPI.Contracts;
using Headlines.WebAPI.Contracts.V1.Models;
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
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.ArticleSources.GetAllResponse>(new()
            {
                ("ArticleSources", typeof(List<ArticleSourceModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_GetSkipTakeResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.HeadlineChanges.GetSkipTakeResponse>(new()
            {
                ("HeadlineChanges", typeof(List<HeadlineChangeModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_GetTopUpvotedResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.HeadlineChanges.GetTopUpvotedResponse>(new()
            {
                ("HeadlineChanges", typeof(List<HeadlineChangeModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_UpvoteResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.HeadlineChanges.UpvoteResponse>(new()
            {
                ("Upvotes", typeof(List<UpvoteModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void HeadlineChanges_GetByArticleIdSkipTakeResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.HeadlineChanges.GetByArticleIdSkipTakeResponse>(new()
            {
                ("HeadlineChanges", typeof(List<HeadlineChangeModel>), new string[] { _jsonPropertyAttribute }),
                ("TotalCount", typeof(long), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void UserUpvotes_GetResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.UserUpvotes.GetResponse>(new()
            {
                ("Upvotes", typeof(List<UpvoteModel>), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void Articles_GetSkipTakeResponse()
        {
            //Assert
            TestExtensions.AssertProperties<WebAPI.Contracts.V1.Responses.Articles.GetSkipTakeResponse>(new()
            {
                ("Articles", typeof(List<ArticleModel>), new string[] { _jsonPropertyAttribute }),
                ("MatchesFiltersCount", typeof(long), new string[] { _jsonPropertyAttribute })
            });
        }
    }
}