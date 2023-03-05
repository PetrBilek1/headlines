using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.WebAPI.Tests.Integration.V1.TestUtils
{
    internal sealed class DatabasePopulator : IAsyncDisposable
    {
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IHeadlineChangeDAO _headlineChangeDAO;
        private readonly IArticleDAO _articleDAO;
        private readonly IArticleSourceDAO _articleSourceDAO;
        private readonly IUserUpvotesDAO _userUpvotesDAO;
        private readonly IMapper _mapper;

        private DatabasePopulator(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            _uowProvider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider>();
            _headlineChangeDAO = scope.ServiceProvider.GetRequiredService<IHeadlineChangeDAO>();
            _articleDAO = scope.ServiceProvider.GetRequiredService<IArticleDAO>();
            _articleSourceDAO = scope.ServiceProvider.GetRequiredService<IArticleSourceDAO>();
            _userUpvotesDAO = scope.ServiceProvider.GetRequiredService<IUserUpvotesDAO>();
            _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        }

        public static async Task<DatabasePopulator> CreateAsync(IServiceProvider serviceProvider)
        {
            var populator = new DatabasePopulator(serviceProvider);

            await populator.ClearDatabaseAsync();

            return populator;
        }

        public async ValueTask DisposeAsync()
        {
            await ClearDatabaseAsync();
        }

        public async Task<List<UserUpvotesDTO>> GetAllUserUpvotesAsync()
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            var upvotes = await _userUpvotesDAO.GetAllAsync(default);

            return _mapper.Map<List<UserUpvotesDTO>>(upvotes);
        }

        public async Task<List<HeadlineChangeDTO>> GetAllHeadlineChangesAsync()
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            var changes = await _headlineChangeDAO.GetAllAsync(default);

            return _mapper.Map<List<HeadlineChangeDTO>>(changes);
        }

        public async Task<List<ArticleSourceDTO>> InsertArticleSourcesAsync(IEnumerable<ArticleSourceDTO> articleSources)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<ArticleSource> inserted = new();

            foreach (ArticleSourceDTO articleSourceDTO in articleSources)
            {
                inserted.Add(await GetOrInsertArticleSourceAsync(articleSourceDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<ArticleSourceDTO>>(inserted);
        }

        public async Task<List<ArticleDTO>> InsertArticlesAsync(IEnumerable<ArticleDTO> articles)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<Article> inserted = new();

            foreach (ArticleDTO articleDTO in articles)
            {
                inserted.Add(await GetOrInsertArticleAsync(articleDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<ArticleDTO>>(inserted);
        }

        public async Task<UserUpvotesDTO> InsertUserUpvotesAsync(UserUpvotesDTO userUpvotes)
        {
            using var uow = _uowProvider.CreateUnitOfWork();

            var inserted = await _userUpvotesDAO.InsertAsync(new ORM.Core.Entities.UserUpvotes
            {
                Id = userUpvotes.Id,
                UserToken = userUpvotes.UserToken,
                Json = userUpvotes.Json,
            });

            await uow.CommitAsync();

            return _mapper.Map<UserUpvotesDTO>(inserted);
        }

        public async Task<List<HeadlineChangeDTO>> InsertHeadlineChangesAsync(IEnumerable<HeadlineChangeDTO> headlineChanges)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<HeadlineChange> inserted = new();

            foreach (HeadlineChangeDTO headlineChangeDTO in headlineChanges)
            {
                inserted.Add(await GetOrInsertHeadlineChangeAsync(headlineChangeDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<HeadlineChangeDTO>>(inserted);
        }

        private async Task ClearDatabaseAsync()
        {
            using (var uow = _uowProvider.CreateUnitOfWork())
            {
                List<HeadlineChange> headlineChanges = await _headlineChangeDAO.GetAllAsync(default);
                headlineChanges.ForEach(_headlineChangeDAO.Delete);

                await uow.CommitAsync();
            }

            using (var uow = _uowProvider.CreateUnitOfWork())
            {
                List<Article> articles = await _articleDAO.GetAllAsync(default);
                articles.ForEach(_articleDAO.Delete);

                await uow.CommitAsync();
            }

            using (var uow = _uowProvider.CreateUnitOfWork())
            {
                List<ArticleSource> articles = await _articleSourceDAO.GetAllAsync(default);
                articles.ForEach(_articleSourceDAO.Delete);

                await uow.CommitAsync();
            }

            using (var uow = _uowProvider.CreateUnitOfWork())
            {
                List<ORM.Core.Entities.UserUpvotes> userUpvotes = await _userUpvotesDAO.GetAllAsync(default);
                userUpvotes.ForEach(_userUpvotesDAO.Delete);

                await uow.CommitAsync();
            }
        }

        private async Task<HeadlineChange> GetOrInsertHeadlineChangeAsync(HeadlineChangeDTO headlineChangeDTO)
        {
            HeadlineChange? headlineChange = headlineChangeDTO.Id != default ? await _headlineChangeDAO.AssertExistsAsync(headlineChangeDTO.Id) : null;

            if (headlineChange != null)
                return headlineChange;

            headlineChange = new HeadlineChange()
            {
                Id = headlineChangeDTO.Id,
                ArticleId = headlineChangeDTO.ArticleId,
                Detected = headlineChangeDTO.Detected,
                TitleBefore = headlineChangeDTO.TitleBefore,
                TitleAfter = headlineChangeDTO.TitleAfter,
                UpvoteCount = headlineChangeDTO.UpvoteCount,
            };

            if (headlineChangeDTO.Article != null)
            {
                headlineChange.Article = await GetOrInsertArticleAsync(headlineChangeDTO.Article);
            }

            await _headlineChangeDAO.InsertAsync(headlineChange);

            return headlineChange;
        }

        private async Task<Article> GetOrInsertArticleAsync(ArticleDTO articleDTO)
        {
            Article? article = articleDTO.Id != default ? await _articleDAO.AssertExistsAsync(articleDTO.Id) : null;

            if (article != null)
                return article;

            article = new Article()
            {
                Id = articleDTO.Id,
                SourceId = articleDTO.SourceId,
                Published = articleDTO.Published,
                UrlId = articleDTO.UrlId,
                CurrentTitle = articleDTO.CurrentTitle,
                Link = articleDTO.Link,
            };

            if (articleDTO.Source != null)
            {
                article.Source = await GetOrInsertArticleSourceAsync(articleDTO.Source);
            }

            await _articleDAO.InsertAsync(article);

            return article;
        }

        private async Task<ArticleSource> GetOrInsertArticleSourceAsync(ArticleSourceDTO articleSourceDTO)
        {
            ArticleSource? articleSource = articleSourceDTO.Id != default ? await _articleSourceDAO.AssertExistsAsync(articleSourceDTO.Id) : null;

            if (articleSource != null)
                return articleSource;

            articleSource = new ArticleSource()
            {
                Id = articleSourceDTO.Id,
                Name = articleSourceDTO.Name,
                RssUrl = articleSourceDTO.RssUrl,
                UrlIdSource = articleSourceDTO.UrlIdSource,
            };

            await _articleSourceDAO.InsertAsync(articleSource);

            return articleSource;
        }
    }
}