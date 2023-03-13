using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class ArticleSourceDao : EfCoreDAO<ArticleSource, long>, IArticleSourceDao
    {
        public ArticleSourceDao(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }

        public Task<List<ArticleSource>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbContext.Set<ArticleSource>()
                .ToListAsync(cancellationToken);
        }
    }
}