using Headlines.ORM.Core.Entities;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class ObjectDataDAO : EfCoreDAO<ObjectData, long>, IObjectDataDAO
    {
        public ObjectDataDAO(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }
    }
}