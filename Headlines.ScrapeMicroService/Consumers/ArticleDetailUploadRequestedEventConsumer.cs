using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using MassTransit;

namespace Headlines.ScrapeMicroService.Consumers
{
    public sealed class ArticleDetailUploadRequestedEventConsumer : IConsumer<ArticleDetailUploadRequestedEvent>
    {
        private const string BucketName = "headlines";

        private readonly IArticleFacade _articleFacade;
        private readonly IObjectStorageWrapper _objectStorage;
        private readonly ILogger<ArticleDetailUploadRequestedEventConsumer> _logger;

        public ArticleDetailUploadRequestedEventConsumer(IArticleFacade articleFacade, IObjectStorageWrapper objectStorage, ILogger<ArticleDetailUploadRequestedEventConsumer> logger)
        {
            _articleFacade = articleFacade;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ArticleDetailUploadRequestedEvent> context)
        {
            _logger.LogInformation("Received request to upload article detail of article with Id '{articleId}'.", context.Message.ArticleId);
            ArgumentNullException.ThrowIfNull(context.Message.Detail);

            try
            {
                ObjectDataDTO objectData = await _objectStorage.UploadObjectAsync(new ArticleDetailDTO
                {
                    IsPaywalled = context.Message.Detail.IsPaywalled,
                    Title = context.Message.Detail.Title,
                    Author = context.Message.Detail.Author,
                    Paragraphs = context.Message.Detail.Paragraphs,
                    Tags = context.Message.Detail.Tags,
                },
                BucketName);

                await _articleFacade.InsertArticleDetailByArticleIdAsync(context.Message.ArticleId, objectData);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There was and exception when trying to upload article detail of article with Id '{articleId}'.", context.Message.ArticleId);
            }
        }
    }
}