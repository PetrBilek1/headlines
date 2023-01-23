using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IArticleSourceDAO : IDAO<ArticleSource, long>
    {
        Task<List<ArticleSource>> GetAllAsync(CancellationToken cancellationToken);
    }
}
