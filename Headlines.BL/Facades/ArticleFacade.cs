using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class ArticleFacade : IArticleFacade
    {
        private readonly IArticleDAO _articleDAO;
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IMapper _mapper;

        public ArticleFacade(IArticleDAO articleDAO, IUnitOfWorkProvider uowProvider, IMapper mapper)
        {
            _articleDAO = articleDAO;
            _uowProvider = uowProvider;
            _mapper = mapper;
        }

        public async Task<List<ArticleDTO>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<Article> articles = await _articleDAO.GetByUrlIdsAsync(ids, cancellationToken);

            return _mapper.Map<List<ArticleDTO>>(articles);
        }

        public async Task<ArticleDTO> CreateOrUpdateArticleAsync(ArticleDTO articleDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            Article article = articleDTO.Id == default
                ? await _articleDAO.InsertAsync(new Article())
                : await _articleDAO.AssertExistsAsync(articleDTO.Id);

            article.SourceId = articleDTO.SourceId;
            article.Published = articleDTO.Published;
            article.UrlId = articleDTO.UrlId;
            article.CurrentTitle = articleDTO.CurrentTitle;
            article.Link = articleDTO.Link;

            await uow.CommitAsync();

            return _mapper.Map<ArticleDTO>(article);
        }

        public async Task<List<ArticleDTO>> GetArticlesByFiltersSkipTakeAsync(int skip, int take, string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<Article> articles = await _articleDAO.GetByFiltersSkipTakeAsync(skip, take, cancellationToken, currentTitlePrompt, articleSources, from, to);

            return _mapper.Map<List<ArticleDTO>>(articles);
        }

        public async Task<long> GetArticlesCountByFiltersAsync(string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            long count = await _articleDAO.GetCountByFiltersAsync(cancellationToken, currentTitlePrompt, articleSources, from, to);

            return count;
        }
    }
}