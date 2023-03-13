using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IHeadlineChangeFacade
    {
        Task<HeadlineChangeDto> CreateOrUpdateHeadlineChangeAsync(HeadlineChangeDto headlineChangeDTO);
        Task<HeadlineChangeDto> DeleteHeadlineChangeAsync(HeadlineChangeDto headlineChangeDTO);
        Task<List<HeadlineChangeDto>> GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync(int take = 10, CancellationToken cancellationToken = default);
        Task<List<HeadlineChangeDto>> GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<long> GetHeadlineChangeCountAsync(long? articleId = null);
        Task<List<HeadlineChangeDto>> GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(long articleId, int skip, int take, CancellationToken cancellationToken = default);
        Task<HeadlineChangeDto> AddUpvotesToHeadlineChangeAsync(long id, int amount);
    }
}