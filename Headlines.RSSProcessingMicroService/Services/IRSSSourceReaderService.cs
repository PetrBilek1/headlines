using Headlines.RSSProcessingMicroService.DTO;

namespace Headlines.RSSProcessingMicroService.Services
{
    public interface IRssSourceReaderService
    {
        Task<List<FeedItemWithArticle>> ReadFeedItemsFromSourcesAsync(CancellationToken cancellationToken = default);
    }
}