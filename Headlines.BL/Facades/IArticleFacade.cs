using Headlines.DTO.Custom;
using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleFacade
    {
        Task<ArticleDto> GetArticleByIdIncludeSourceAsync(long id, CancellationToken cancellationToken = default);
        Task<ArticleDto> GetArticleByIdIncludeDetailsAsync(long id, CancellationToken cancellationToken = default);
        Task<List<ArticleDto>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default);
        Task<ArticleDto> CreateOrUpdateArticleAsync(ArticleDto articleDTO);
        Task<ObjectDataDto> InsertArticleDetailByArticleIdAsync(long articleId, ObjectDataDto dataDTO);
        Task<List<ArticleDto>> GetArticlesByFiltersSkipTakeAsync(int skip, int take, string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<long> GetArticlesCountByFiltersAsync(string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    }
}