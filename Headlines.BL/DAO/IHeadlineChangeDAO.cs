using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IHeadlineChangeDAO : IDAO<HeadlineChange, long>
    {
        Task<List<HeadlineChange>> GetOrderByUpvotesCountIncludeArticleAsync(int take, CancellationToken cancellationToken);
        Task<List<HeadlineChange>> GetOrderByDetectedDescendingIncludeArticleAsync(int skip, int take, CancellationToken cancellationToken);
        Task<long> GetCountAsync(long? articleId = null);
        Task<List<HeadlineChange>> GetByArticleIdOrderByDetectedDescendingAsync(long articleId, int skip, int take, CancellationToken cancellationToken);
        Task<List<HeadlineChange>> GetAllAsync(CancellationToken cancellationToken);
    }
}