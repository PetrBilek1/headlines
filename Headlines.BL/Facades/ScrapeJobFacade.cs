using Headlines.BL.DAO;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class ScrapeJobFacade : IScrapeJobFacade
    {
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IScrapeJobDAO _scrapeJobDAO;

        public ScrapeJobFacade(IUnitOfWorkProvider uowProvider, IScrapeJobDAO scrapeJobDAO)
        {
            _uowProvider = uowProvider;
            _scrapeJobDAO = scrapeJobDAO;
        }
    }
}