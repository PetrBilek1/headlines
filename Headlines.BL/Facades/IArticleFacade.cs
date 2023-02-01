using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleFacade
    {
        Task<List<ArticleDTO>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default);
        Task<ArticleDTO> CreateOrUpdateArticleAsync(ArticleDTO articleDTO);
        Task<List<ArticleDTO>> GetArticlesByFiltersSkipTakeAsync(int skip, int take, string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<long> GetArticlesCountByFiltersAsync(string? currentTitlePrompt = null, long[]? articleSources = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    }
}