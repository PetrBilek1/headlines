using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IScrapeJobFacade
    {
        Task<ScrapeJobDTO> CreateOrUpdateScrapeJobAsync(ScrapeJobDTO scrapeJobDTO);
    }
}