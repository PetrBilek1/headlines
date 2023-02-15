using Headlines.DTO.Custom;
using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleFacade
    {
        Task<ArticleDTO> GetArticleByIdIncludeSourceAsync(long id, CancellationToken cancellationToken = default);
        Task<List<ArticleDTO>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default);
        Task<ArticleDTO> CreateOrUpdateArticleAsync(ArticleDTO articleDTO);
        Task<ObjectDataDTO> InsertArticleDetailByArticleIdAsync(long articleId, ObjectDataDTO dataDTO);
        Task<List<ArticleDTO>> GetArticlesByFiltersSkipTakeAsync(int skip, int take, string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<long> GetArticlesCountByFiltersAsync(string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    }
}