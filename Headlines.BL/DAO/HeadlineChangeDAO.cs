using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class HeadlineChangeDao : EfCoreDAO<HeadlineChange, long>, IHeadlineChangeDao
    {
        public HeadlineChangeDao(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }

        public Task<List<HeadlineChange>> GetOrderByUpvotesCountIncludeArticleAsync(int take, CancellationToken cancellationToken)
        {
            return DbContext.Set<HeadlineChange>()
                .OrderByDescending(x => x.UpvoteCount)
                .ThenByDescending(x => x.Detected)
                .Include(x => x.Article)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<List<HeadlineChange>> GetOrderByDetectedDescendingIncludeArticleAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return DbContext.Set<HeadlineChange>()
                .OrderByDescending(x => x.Detected)
                .Include(x => x.Article)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<long> GetCountAsync(long? articleId = null)
        {
            return DbContext.Set<HeadlineChange>()
                .Where(x => !articleId.HasValue || x.ArticleId == articleId)
                .LongCountAsync();
        }

        public Task<List<HeadlineChange>> GetByArticleIdOrderByDetectedDescendingAsync(long articleId, int skip, int take, CancellationToken cancellationToken)
        {
            return DbContext.Set<HeadlineChange>()
                .OrderByDescending(x => x.Detected)
                .Where(x => x.ArticleId == articleId)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<List<HeadlineChange>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbContext.Set<HeadlineChange>()
                .ToListAsync(cancellationToken);
        }
    }
}