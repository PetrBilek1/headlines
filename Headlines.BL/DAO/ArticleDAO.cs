using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class ArticleDAO : EfCoreDAO<Article, long>, IArticleDAO
    {
        public ArticleDAO(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }

        public Task<List<Article>> GetByUrlIdsAsync(string[] urlIds, CancellationToken cancellationToken)
        {
            return DbContext.Set<Article>()
                .Where(x => urlIds.Contains(x.UrlId))
                .ToListAsync(cancellationToken);
        }

        public Task<List<Article>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbContext.Set<Article>()
                .ToListAsync(cancellationToken);
        }
    }
}