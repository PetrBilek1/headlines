using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleSourceFacade
    {
        Task<List<ArticleSourceDTO>> GetAllArticleSourcesAsync(CancellationToken cancellationToken = default);
    }
}
