using FluentAssertions;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;

namespace Headlines.BL.Tests
{
    public static class AssertionUtils
    {
        public static void AssertObjectDataMatches(ObjectDataDto expected, ObjectDataDto actual)
        {
            actual.Id.Should().Be(expected.Id);
            actual.Bucket.Should().Be(expected.Bucket);
            actual.Key.Should().Be(expected.Key);
            actual.ContentType.Should().Be(expected.ContentType);
            actual.Created.Should().Be(expected.Created);
            actual.Changed.Should().Be(expected.Changed);
        }
        public static void AssertObjectDataMatches(ObjectDataDto expected, ObjectData actual)
        {
            actual.Id.Should().Be(expected.Id);
            actual.Bucket.Should().Be(expected.Bucket);
            actual.Key.Should().Be(expected.Key);
            actual.ContentType.Should().Be(expected.ContentType);
            actual.Created.Should().Be(expected.Created);
            actual.Changed.Should().Be(expected.Changed);
        }
    }
}