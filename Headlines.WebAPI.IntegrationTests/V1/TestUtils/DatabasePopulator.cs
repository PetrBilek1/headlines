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
        private readonly IHeadlineChangeDao _headlineChangeDAO;
        private readonly IArticleDao _articleDAO;
        private readonly IArticleSourceDao _articleSourceDAO;
        private readonly IUserUpvotesDao _userUpvotesDAO;
        private readonly IObjectDataDao _objectDataDAO;
        private readonly IMapper _mapper;

        private DatabasePopulator(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            _uowProvider = scope.ServiceProvider.GetRequiredService<IUnitOfWorkProvider>();
            _headlineChangeDAO = scope.ServiceProvider.GetRequiredService<IHeadlineChangeDao>();
            _articleDAO = scope.ServiceProvider.GetRequiredService<IArticleDao>();
            _articleSourceDAO = scope.ServiceProvider.GetRequiredService<IArticleSourceDao>();
            _userUpvotesDAO = scope.ServiceProvider.GetRequiredService<IUserUpvotesDao>();
            _objectDataDAO = scope.ServiceProvider.GetRequiredService<IObjectDataDao>();
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

        public async Task<List<UserUpvotesDto>> GetAllUserUpvotesAsync()
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            var upvotes = await _userUpvotesDAO.GetAllAsync(default);

            return _mapper.Map<List<UserUpvotesDto>>(upvotes);
        }

        public async Task<List<HeadlineChangeDto>> GetAllHeadlineChangesAsync()
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            var changes = await _headlineChangeDAO.GetAllAsync(default);

            return _mapper.Map<List<HeadlineChangeDto>>(changes);
        }

        public async Task<List<ArticleSourceDto>> InsertArticleSourcesAsync(IEnumerable<ArticleSourceDto> articleSources)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<ArticleSource> inserted = new();

            foreach (ArticleSourceDto articleSourceDTO in articleSources)
            {
                inserted.Add(await GetOrInsertArticleSourceAsync(articleSourceDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<ArticleSourceDto>>(inserted);
        }

        public async Task<List<ArticleDto>> InsertArticlesAsync(IEnumerable<ArticleDto> articles)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<Article> inserted = new();

            foreach (ArticleDto articleDTO in articles)
            {
                inserted.Add(await GetOrInsertArticleAsync(articleDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<ArticleDto>>(inserted);
        }

        public async Task<UserUpvotesDto> InsertUserUpvotesAsync(UserUpvotesDto userUpvotes)
        {
            using var uow = _uowProvider.CreateUnitOfWork();

            var inserted = await _userUpvotesDAO.InsertAsync(new ORM.Core.Entities.UserUpvotes
            {
                Id = userUpvotes.Id,
                UserToken = userUpvotes.UserToken,
                Json = userUpvotes.Json,
            });

            await uow.CommitAsync();

            return _mapper.Map<UserUpvotesDto>(inserted);
        }

        public async Task<List<HeadlineChangeDto>> InsertHeadlineChangesAsync(IEnumerable<HeadlineChangeDto> headlineChanges)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            List<HeadlineChange> inserted = new();

            foreach (HeadlineChangeDto headlineChangeDTO in headlineChanges)
            {
                inserted.Add(await GetOrInsertHeadlineChangeAsync(headlineChangeDTO));
            }

            await uow.CommitAsync();

            return _mapper.Map<List<HeadlineChangeDto>>(inserted);
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

            using (var uow = _uowProvider.CreateUnitOfWork())
            {
                List<ObjectData> objects = await _objectDataDAO.GetAllAsync(default);
                objects.ForEach(_objectDataDAO.Delete);

                await uow.CommitAsync();
            }
        }

        private async Task<HeadlineChange> GetOrInsertHeadlineChangeAsync(HeadlineChangeDto headlineChangeDTO)
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

        private async Task<Article> GetOrInsertArticleAsync(ArticleDto articleDTO)
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

            if (articleDTO.Details != null)
            {
                articleDTO.Details.ForEach(async x =>
                {
                    article.Details.Add(await GetOrInsertObjectDataAsync(x));
                });
            }

            await _articleDAO.InsertAsync(article);

            return article;
        }

        private async Task<ArticleSource> GetOrInsertArticleSourceAsync(ArticleSourceDto articleSourceDTO)
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
                ScraperType = articleSourceDTO.ScraperType,
            };

            await _articleSourceDAO.InsertAsync(articleSource);

            return articleSource;
        }

        private async Task<ObjectData> GetOrInsertObjectDataAsync(ObjectDataDto objectDataDTO)
        {
            ObjectData? objectData = objectDataDTO.Id != default ? await _objectDataDAO.AssertExistsAsync(objectDataDTO.Id) : null;

            if (objectData != null)
                return objectData;

            objectData = new ObjectData()
            {
                Id = objectDataDTO.Id,
                Bucket = objectDataDTO.Bucket,
                Key = objectDataDTO.Key,
                ContentType = objectDataDTO.ContentType,
                Created = objectDataDTO.Created,
                Changed = objectDataDTO.Changed,
            };

            await _objectDataDAO.InsertAsync(objectData);

            return objectData;
        }
    }
}