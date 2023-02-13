using Headlines.BL.Facades;

namespace Headlines.ScrapeMicroService.Services
{
    public sealed class ScrapeJobProvider : IScrapeJobProvider
    {
        private readonly IScrapeJobFacade _scrapeJobFacade;

        public ScrapeJobProvider(IScrapeJobFacade scrapeJobFacade) 
        {
            _scrapeJobFacade = scrapeJobFacade;
        }

    }
}