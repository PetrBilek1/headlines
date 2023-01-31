using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;
using System.Linq;

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

        public Task<List<Article>> GetByFiltersSkipTakeAsync(int skip, int take, string currentTitlePrompt, CancellationToken cancellationToken, long[]? articleSources = null)
        {
            return GetByFiltersSkipTakeQueryable(skip, take, currentTitlePrompt, articleSources)
                .ToListAsync(cancellationToken);
        }

        public Task<long> GetCountByFiltersSkipTakeAsync(int skip, int take, string currentTitlePrompt, CancellationToken cancellationToken, long[]? articleSources = null)
        {
            return GetByFiltersSkipTakeQueryable(null, null, currentTitlePrompt, articleSources)
                .LongCountAsync(cancellationToken);
        }

        private IQueryable<Article> GetByFiltersSkipTakeQueryable(int? skip, int? take, string currentTitlePrompt, long[]? articleSources)
        {
            IQueryable<Article> query = DbContext.Set<Article>().AsQueryable();

            if (!string.IsNullOrEmpty(currentTitlePrompt))
            {
                query = query.Where(x => EF.Functions.FreeText(x.CurrentTitle, currentTitlePrompt));
            }

            if (articleSources != null)
            {
                query = query.Where(x => articleSources.Contains(x.SourceId));
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);

            }

            return query;
        }
    }
}