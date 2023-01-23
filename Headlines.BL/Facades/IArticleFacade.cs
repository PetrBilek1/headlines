using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleFacade
    {
        Task<List<ArticleDTO>> GetArticlesByUrlIdsAsync(string[] ids, CancellationToken cancellationToken = default);
        Task<ArticleDTO> CreateOrUpdateArticleAsync(ArticleDTO articleDTO);
    }
}