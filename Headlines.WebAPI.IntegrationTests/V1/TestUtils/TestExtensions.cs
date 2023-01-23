using FluentAssertions;
using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace Headlines.WebAPI.Tests.Integration.V1.TestUtils
{
    internal static class TestExtensions
    {
        public static void AssertProperties<T>(List<(string Name, Type Type, string[] Attributes)> expectedProperties)
        {
            typeof(T).GetProperties().Should().HaveCount(expectedProperties.Count);

            foreach(var expected in expectedProperties)
            {
                var currentProperty = typeof(T).GetPropertyByName(expected.Name);

                currentProperty.Should().NotBeNull();
                currentProperty.PropertyType.Should().Be(expected.Type);

                currentProperty.CustomAttributes.Should().HaveCount(expected.Attributes?.Count() ?? 0);
                foreach(var attribute in expected.Attributes ?? new string[0])
                {
                    currentProperty.CustomAttributes
                        .FirstOrDefault(x => x.AttributeType.FullName == attribute)
                        .Should()
                        .NotBeNull();
                }
            }
        }

        public static PropertyInfo GetPropertyByName(this Type type, string name)
        {
            return type.GetProperties().First(x => x.Name == name);
        }

        public static string UpvoteListToJson(List<UpvoteModel> upvotes)
        {
            return JsonConvert.SerializeObject(upvotes, Formatting.None);
        }

        public static List<UpvoteModel> JsonToUpvoteList(string json)
        {
            return JsonConvert.DeserializeObject<List<UpvoteModel>>(json) ?? new List<UpvoteModel>();
        }
    }
}