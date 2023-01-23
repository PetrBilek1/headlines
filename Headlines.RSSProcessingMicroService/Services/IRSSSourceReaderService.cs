using Headlines.RSSProcessingMicroService.DTO;

namespace Headlines.RSSProcessingMicroService.Services
{
    public interface IRSSSourceReaderService
    {
        Task<List<FeedItemWithArticle>> ReadFeedItemsFromSourcesAsync(CancellationToken cancellationToken = default);
    }
}