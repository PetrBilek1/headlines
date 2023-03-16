using FluentAssertions;
using Headlines.BL.Implementations.ObjectStorage;
using Headlines.DTO.Entities;
using Moq;
using Newtonsoft.Json;
using PBilek.ObjectStorageService;
using System.Text;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ObjectStorage
{
    public sealed class ObjectStorageWrapperTests
    {
        private readonly Mock<IObjectStorageService> _objectStorageMock = new(MockBehavior.Strict);

        private readonly ObjectStorageWrapper _sut;

        public ObjectStorageWrapperTests()
        {
            _sut = new ObjectStorageWrapper(_objectStorageMock.Object);
        }

        [Fact]
        public async Task UploadObjectAsync_Simple()
        {
            //Arrange
            string bucket = "bucket";
            string contentType = "application/json";

            _objectStorageMock.Setup(x => x.PutObjectAsync(bucket, It.IsAny<string>(), contentType, It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);    

            //Act
            ObjectDataDto result = await _sut.UploadObjectAsync(new InputData { Content = "content" }, bucket, default);

            //Assert
            result.Should().NotBeNull();
            result.Bucket.Should().Be(bucket);
            result.Key.Should().NotBeNull();
            Guid.TryParse(result.Key.Split("/")[1].Replace(".json", ""), out _).Should().BeTrue();
            result.ContentType.Should().Be(contentType);

            _objectStorageMock.Verify(x => x.PutObjectAsync(bucket, result.Key, contentType, It.IsAny<MemoryStream>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DownloadObjectAsync_Simple()
        {
            //Arrange
            string bucket = "bucket";
            string key = "key";
            using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new InputData { Content = "content" })));

            _objectStorageMock.Setup(x => x.DownloadObjectAsync(bucket, key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stream);

            //Act
            var result = await _sut.DownloadObjectAsync<InputData>(bucket, key, default);

            //Assert
            result.Should().NotBeNull();
            result!.Content.Should().Be("content");
        }

        private sealed class InputData
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}