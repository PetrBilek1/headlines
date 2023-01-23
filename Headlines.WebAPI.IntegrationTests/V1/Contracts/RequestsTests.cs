using FluentAssertions;
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

            var requests = typeof(IApiMarker).Assembly
                .GetTypes()
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("Headlines.WebAPI.Contracts.V1.Requests"))
                .Select(x => x.Name)
                .ToList();

            //Assert
            foreach (var request in requests)
            {
                tests.Should().Contain(request);
            }
        }

        [Fact]
        public void UpvoteRequest()
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