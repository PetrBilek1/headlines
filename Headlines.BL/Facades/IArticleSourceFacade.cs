using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IArticleSourceFacade
    {
        Task<List<ArticleSourceDto>> GetAllArticleSourcesAsync(CancellationToken cancellationToken = default);
    }
}
