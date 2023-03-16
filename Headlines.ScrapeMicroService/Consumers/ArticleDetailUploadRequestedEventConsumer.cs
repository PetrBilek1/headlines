using Amazon.S3;
using Headlines.BL.Abstractions.ObjectStorage;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.ScrapeMicroService.Configuration;
using MassTransit;

namespace Headlines.ScrapeMicroService.Consumers
{
    public sealed class ArticleDetailUploadRequestedEventConsumer : IConsumer<ArticleDetailUploadRequestedEvent>
    {
        private readonly IArticleFacade _articleFacade;
        private readonly IObjectStorageWrapper _objectStorage;
        private readonly ILogger<ArticleDetailUploadRequestedEventConsumer> _logger;
        private readonly UploadConfiguration _uploadConfiguration;

        public ArticleDetailUploadRequestedEventConsumer(IArticleFacade articleFacade, IObjectStorageWrapper objectStorage, ILogger<ArticleDetailUploadRequestedEventConsumer> logger, UploadConfiguration uploadConfiguration)
        {
            _articleFacade = articleFacade;
            _objectStorage = objectStorage;
            _logger = logger;
            _uploadConfiguration = uploadConfiguration;
        }

        public async Task Consume(ConsumeContext<ArticleDetailUploadRequestedEvent> context)
        {
            _logger.LogInformation("Received request to upload article detail of article with Id '{articleId}'.", context.Message.ArticleId);
            ArgumentNullException.ThrowIfNull(context.Message.Detail);

            try
            {
                ObjectDataDto objectData = await _objectStorage.UploadObjectAsync(new ArticleDetailDto
                {
                    IsPaywalled = context.Message.Detail.IsPaywalled,
                    Title = context.Message.Detail.Title,
                    Author = context.Message.Detail.Author,
                    Paragraphs = context.Message.Detail.Paragraphs,
                    Tags = context.Message.Detail.Tags,
                },
                _uploadConfiguration.BucketName);

                await _articleFacade.InsertArticleDetailByArticleIdAsync(context.Message.ArticleId, objectData);
            }
            catch(Exception e)
            {
                if (e.GetType() == typeof(AmazonS3Exception) && (e as AmazonS3Exception)?.ErrorCode == "TooManyRequests")
                {
                    _logger.LogWarning(e, "There was TooManyRequests exception when trying to upload article detail of article with Id '{articleId}'. Retrying.", context.Message.ArticleId);
                    await context.SchedulePublish(TimeSpan.FromSeconds(1), context.Message);
                    return;
                }

                _logger.LogError(e, "There was and exception when trying to upload article detail of article with Id '{articleId}'.", context.Message.ArticleId);
                throw;
            }
        }
    }
}