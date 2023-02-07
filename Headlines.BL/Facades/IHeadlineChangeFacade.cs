using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IHeadlineChangeFacade
    {
        Task<HeadlineChangeDTO> CreateOrUpdateHeadlineChangeAsync(HeadlineChangeDTO headlineChangeDTO);
        Task<HeadlineChangeDTO> DeleteHeadlineChangeAsync(HeadlineChangeDTO headlineChangeDTO);
        Task<List<HeadlineChangeDTO>> GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync(int take = 10, CancellationToken cancellationToken = default);
        Task<List<HeadlineChangeDTO>> GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<long> GetHeadlineChangeCountAsync(long? articleId = null);
        Task<List<HeadlineChangeDTO>> GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(long articleId, int skip, int take, CancellationToken cancellationToken = default);
        Task<HeadlineChangeDTO> AddUpvotesToHeadlineChangeAsync(long id, int amount);
    }
}