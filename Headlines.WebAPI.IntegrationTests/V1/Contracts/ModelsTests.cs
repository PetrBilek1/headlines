﻿using FluentAssertions;
using Headlines.Enums;
using Headlines.WebAPI.Contracts;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
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

            var models = typeof(IApiContractsMarker).Assembly
                .GetTypes()
                .Where(x => x.Namespace == "Headlines.WebAPI.Contracts.V1.Models")
                .Select(x => x.Name)
                .ToList();

            //Assert
            models.Should().HaveCountGreaterThan(0);

            foreach(var model in models)
            {
                tests.Should().Contain(model);
            }
        }

        [Fact]
        public void ArticleSourceModel()
        {
            //Assert
            TestExtensions.AssertProperties<ArticleSourceModel>(new()
            {
                ("Id", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Name", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("ScrapingSupported", typeof(bool), new string[] { _jsonPropertyAttribute }),
            });
        }

        [Fact]
        public void ArticleModel()
        {
            //Assert
            TestExtensions.AssertProperties<ArticleModel>(new()
            {
                ("Id", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("SourceId", typeof(long), new string[] { _jsonPropertyAttribute }),
                ("Published", typeof(DateTime?), new string[] { _jsonPropertyAttribute }),
                ("UrlId", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("CurrentTitle", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("Link", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("Source", typeof(ArticleSourceModel), new string[] { _jsonPropertyAttribute }),
            });
        }

        [Fact]
        public void ArticleDetailModel()
        {
            //Assert
            TestExtensions.AssertProperties<ArticleDetailModel>(new()
            {
                ("IsPaywalled", typeof(bool), new string[] { _jsonPropertyAttribute }),
                ("Title", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("Author", typeof(string), new string[] { _jsonPropertyAttribute }),
                ("Paragraphs", typeof(List<string>), new string[] { _jsonPropertyAttribute }),
                ("Tags", typeof(List<string>), new string[] { _jsonPropertyAttribute }),
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