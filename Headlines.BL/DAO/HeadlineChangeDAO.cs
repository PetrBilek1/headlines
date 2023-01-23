using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class HeadlineChangeDAO : EfCoreDAO<HeadlineChange, long>, IHeadlineChangeDAO
    {
        public HeadlineChangeDAO(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
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

        public Task<long> GetCountAsync()
        {
            return DbContext.Set<HeadlineChange>()
                .LongCountAsync();
        }

        public Task<List<HeadlineChange>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbContext.Set<HeadlineChange>()
                .ToListAsync(cancellationToken);
        }
    }
}