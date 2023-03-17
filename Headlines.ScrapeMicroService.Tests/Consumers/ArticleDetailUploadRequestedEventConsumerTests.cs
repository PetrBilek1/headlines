using Headlines.BL.Abstractions.ObjectStorage;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.ScrapeMicroService.Configuration;
using Headlines.ScrapeMicroService.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Headlines.ScrapeMicroService.Tests.Consumers
{
    public sealed class ArticleDetailUploadRequestedEventConsumerTests
    {
        private const string BucketName = "bucketName";

        private readonly ArticleDetailUploadRequestedEventConsumer _sut;

        private readonly Mock<ConsumeContext<ArticleDetailUploadRequestedEvent>> _consumeContextMock = new Mock<ConsumeContext<ArticleDetailUploadRequestedEvent>>();

        private readonly Mock<IArticleFacade> _articleFacadeMock = new(MockBehavior.Strict);
        private readonly Mock<IObjectStorageWrapper> _objectStorageMock = new(MockBehavior.Strict);
        private readonly Mock<ILogger<ArticleDetailUploadRequestedEventConsumer>> _loggerMock = new Mock<ILogger<ArticleDetailUploadRequestedEventConsumer>>();

        public ArticleDetailUploadRequestedEventConsumerTests()
        {
            _sut = new ArticleDetailUploadRequestedEventConsumer(_articleFacadeMock.Object, _objectStorageMock.Object, _loggerMock.Object, new UploadConfiguration { BucketName = BucketName });
        }

        [Fact]
        public async Task Consume_Simple()
        {
            //Arrange
            long articleId = 10;
            ObjectDataDto objectData = new()
            {
                Bucket = BucketName,
                Key = "key",
                ContentType = "application/json",
                Created = new DateTime(2020, 10, 10)
            };

            _objectStorageMock.Setup(x => x.UploadObjectAsync(It.IsAny<ArticleDetailDto>(), BucketName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(objectData);

            _articleFacadeMock.Setup(x => x.InsertArticleDetailByArticleIdAsync(articleId, objectData))
                .ReturnsAsync(objectData);

            _consumeContextMock.Setup(x => x.Message)
                .Returns(new ArticleDetailUploadRequestedEvent
                {
                    ArticleId = articleId,
                    Detail = new ArticleDetailDto()
                });

            //Act
            await _sut.Consume(_consumeContextMock.Object);

            //Assert
            _objectStorageMock.Verify(x => x.UploadObjectAsync(It.IsAny<ArticleDetailDto>(), BucketName, It.IsAny<CancellationToken>()), Times.Once);
            _articleFacadeMock.Verify(x => x.InsertArticleDetailByArticleIdAsync(articleId, objectData), Times.Once);
        }
    }
}