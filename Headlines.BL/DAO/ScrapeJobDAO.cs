using Headlines.ORM.Core.Entities;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class ScrapeJobDAO : EfCoreDAO<ScrapeJob, long>, IScrapeJobDAO
    {
        public ScrapeJobDAO(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }
    }
}