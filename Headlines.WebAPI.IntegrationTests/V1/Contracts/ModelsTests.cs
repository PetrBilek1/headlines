using FluentAssertions;
using Headlines.Enums;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using System.Reflection;
using Xunit;

namespace Headlines.WebAPI.Tests.Integration.V1.Contracts
{
    public sealed class ModelsTests
    {
        private readonly string _jsonPropertyAttribute = "Newtonsoft.Json.JsonPropertyAttribute";

        [Fact]
        public void ShouldCoverAllModelsByTests()
        {
            //Arrange
            var tests = typeof(ModelsTests)
                .GetMethods()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType.FullName == "Xunit.FactAttribute"))
                .Select(x => x.Name)
                .ToHashSet();

            var models = typeof(IApiMarker).Assembly
                .GetTypes()
                .Where(x => x.Namespace == "Headlines.WebAPI.Contracts.V1.Models")
                .Select(x => x.Name)
                .ToList();

            //Assert
            foreach(var model in models)
            {
                tests.Should().Contain(model);
            }
        }

        [Fact]
        public void ArticleModel()
        {
            //Assert
            TestExtensions.AssertProperties<ArticleModel>(new()
            {
                ("Id", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Published", typeof(DateTime?), new string[] { _jsonPropertyAttribute }),
                ("UrlId", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("CurrentTitle", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("Link", typeof(string), new string[] { _jsonPropertyAttribute }),
            });
        }

        [Fact]
        public void HeadlineChangeModel()
        {
            //Assert
            TestExtensions.AssertProperties<HeadlineChangeModel>(new()
            {
                ("Id", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("ArticleId", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Detected", typeof(DateTime), new string[] { _jsonPropertyAttribute }),
                ("TitleBefore", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("TitleAfter", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("UpvoteCount", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Article", typeof(ArticleModel), new string[] { _jsonPropertyAttribute })
            });
        }

        [Fact]
        public void UpvoteModel() 
        {
            //Assert
            TestExtensions.AssertProperties<UpvoteModel>(new()
            {
                ("Type", typeof(UpvoteType), new string[] { _jsonPropertyAttribute }),
                ("TargetId", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Date", typeof(DateTime), new string[] { _jsonPropertyAttribute })
            });
        }              
    }
}