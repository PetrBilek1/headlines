using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.RSSProcessingMicroService.DTO;
using Headlines.RSSProcessingMicroService.Utils;
using PBilek.Infrastructure.DatetimeProvider;

namespace Headlines.RSSProcessingMicroService.Services
{
    public sealed class RssProcessorService : IRssProcessorService
    {
        private readonly IRssSourceReaderService _sourceReaderService;
        private readonly IArticleFacade _articleFacade;
        private readonly IHeadlineChangeFacade _headlineChangeFacade;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RssProcessorService(IRssSourceReaderService sourceReaderService, IArticleFacade articleFacade, IHeadlineChangeFacade headlineChangeFacade, IDateTimeProvider dateTimeProvider)
        {
            _sourceReaderService = sourceReaderService;
            _articleFacade = articleFacade;
            _headlineChangeFacade = headlineChangeFacade;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ProcessingResultDto> DoWorkAsync(CancellationToken cancellationToken = default)
        {
            List<FeedItemWithArticle> feedItems = await _sourceReaderService.ReadFeedItemsFromSourcesAsync(cancellationToken);
            var result = new ProcessingResultDto();

            foreach (FeedItemWithArticle group in feedItems)
            {
                bool createdArticle = CreateArticleIfNull(group);                

                if (group.Article!.CurrentTitle != group.FeedItem?.Title)
                {
                    await RecordHeadlineChangeAsync(group, result);
                }

                ArticleDto article = await _articleFacade.CreateOrUpdateArticleAsync(group.Article);
                AddArticleToResult(result, article, createdArticle);
            }

            return result;
        }    

        private async Task RecordHeadlineChangeAsync(FeedItemWithArticle feedItem, ProcessingResultDto result)
        {
            HeadlineChangeDto change = await _headlineChangeFacade.CreateOrUpdateHeadlineChangeAsync(new HeadlineChangeDto
            {
                ArticleId = feedItem.Article!.Id,
                Detected = _dateTimeProvider.Now,
                TitleBefore = feedItem.Article!.CurrentTitle,
                TitleAfter = feedItem.FeedItem!.Title
            });

            result.RecordedHeadlineChanges.Add(change);

            feedItem.Article.CurrentTitle = feedItem.FeedItem.Title;
            feedItem.Article.Link = feedItem.FeedItem.Link;
        }

        private void AddArticleToResult(ProcessingResultDto result, ArticleDto article, bool wasCreated)
        {
            if (wasCreated)
            {
                result.CreatedArticles.Add(article);
                return;
            }

            result.UpdatedArticles.Add(article);
        }
        
        private bool CreateArticleIfNull(FeedItemWithArticle group)
        {
            if (group.Article != null)
                return false;

            group.Article = new ArticleDto()
            {
                SourceId = group.ArticleSource!.Id,
                Published = group.FeedItem!.Published ?? _dateTimeProvider.Now,
                UrlId = FeedItemUtils.GetUrlId(group.FeedItem, group.ArticleSource),
                CurrentTitle = group.FeedItem.Title,
                Link = group.FeedItem.Link
            };

            return true;
        }
    }
}