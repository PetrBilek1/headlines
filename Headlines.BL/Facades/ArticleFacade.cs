using AutoMapper;
using Headlines.BL.DAO;
using Headlines.BL.Exceptions;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class ArticleFacade : IArticleFacade
    {

        private readonly IArticleDao _articleDao;
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IMapper _mapper;

        public ArticleFacade(IArticleDao articleDAO, IUnitOfWorkProvider uowProvider, IMapper mapper)
        {
            _articleDao = articleDAO;
            _uowProvider = uowProvider;
            _mapper = mapper;
        }

        public async Task<ArticleDto> GetArticleByIdIncludeSourceAsync(long id, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            Article article = await _articleDao.GetByIdAsync(id, cancellationToken, x => x.Source);

            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto> GetArticleByIdIncludeDetailsAsync(long id, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            Article article = await _articleDao.GetByIdAsync(id, cancellationToken, x => x.Details);

            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<List<ArticleDto>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<Article> articles = await _articleDao.GetByUrlIdsAsync(ids, cancellationToken);

            return _mapper.Map<List<ArticleDto>>(articles);
        }

        public async Task<ArticleDto> CreateOrUpdateArticleAsync(ArticleDto articleDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            Article article = articleDTO.Id == default
                ? await _articleDao.InsertAsync(new Article())
                : await _articleDao.AssertExistsAsync(articleDTO.Id);

            article.SourceId = articleDTO.SourceId;
            article.Published = articleDTO.Published;
            article.UrlId = articleDTO.UrlId;
            article.CurrentTitle = articleDTO.CurrentTitle;
            article.Link = articleDTO.Link;

            await uow.CommitAsync();

            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ObjectDataDto> InsertArticleDetailByArticleIdAsync(long articleId, ObjectDataDto dataDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            var article = await _articleDao.GetByIdAsync(articleId);
            if (article == null)
                throw new ResourceNotFoundException($"Article with Id '{articleId}' not found.");

            var data = new ObjectData
            {
                Bucket = dataDTO.Bucket,
                Key = dataDTO.Key,
                ContentType = dataDTO.ContentType,
            };
            article.Details.Add(data);

            await uow.CommitAsync();

            return _mapper.Map<ObjectDataDto>(data);
        }

        public async Task<List<ArticleDto>> GetArticlesByFiltersSkipTakeAsync(int skip, int take, string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<Article> articles = await _articleDao.GetByFiltersSkipTakeAsync(skip, take, cancellationToken, currentTitlePrompt, articleSources, from, to);

            return _mapper.Map<List<ArticleDto>>(articles);
        }

        public async Task<long> GetArticlesCountByFiltersAsync(string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            long count = await _articleDao.GetCountByFiltersAsync(cancellationToken, currentTitlePrompt, articleSources, from, to);

            return count;
        }
    }
}