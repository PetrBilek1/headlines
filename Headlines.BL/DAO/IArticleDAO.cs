using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IArticleDAO : IDAO<Article, long>
    {
        Task<List<Article>> GetByUrlIdsAsync(string[] urlIds, CancellationToken cancellationToken);
        Task<List<Article>> GetAllAsync(CancellationToken cancellationToken);
    }
}