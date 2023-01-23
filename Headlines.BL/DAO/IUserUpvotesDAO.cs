using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IUserUpvotesDAO : IDAO<UserUpvotes, long>
    {
        Task<UserUpvotes> GetByUserTokenAsync(string token, CancellationToken cancellationToken);
        Task<List<UserUpvotes>> GetAllAsync(CancellationToken cancellationToken);
    }
}