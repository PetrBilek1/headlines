using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
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

        public Task<List<ObjectData>> GetAllAsync(CancellationToken cancellationToken) 
        { 
            return DbContext.Set<ObjectData>()
                .ToListAsync(cancellationToken);
        }
    }
}