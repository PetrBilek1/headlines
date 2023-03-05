using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IObjectDataDAO : IDAO<ObjectData, long>
    {
        Task<List<ObjectData>> GetAllAsync(CancellationToken cancellationToken);
    }
}