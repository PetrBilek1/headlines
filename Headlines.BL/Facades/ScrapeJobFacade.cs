using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class ScrapeJobFacade : IScrapeJobFacade
    {
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IScrapeJobDAO _scrapeJobDAO;
        private readonly IMapper _mapper;

        public ScrapeJobFacade(IUnitOfWorkProvider uowProvider, IScrapeJobDAO scrapeJobDAO, IMapper mapper)
        {
            _uowProvider = uowProvider;
            _scrapeJobDAO = scrapeJobDAO;
            _mapper = mapper;
        }

        public async Task<ScrapeJobDTO> CreateOrUpdateScrapeJobAsync(ScrapeJobDTO scrapeJobDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            ScrapeJob scrapeJob = scrapeJobDTO.Id == default
                ? await _scrapeJobDAO.InsertAsync(new ScrapeJob())
                : await _scrapeJobDAO.AssertExistsAsync(scrapeJobDTO.Id);

            scrapeJob.ArticleId = scrapeJobDTO.ArticleId;
            scrapeJob.Priority = scrapeJobDTO.Priority;

            await uow.CommitAsync();

            return _mapper.Map<ScrapeJobDTO>(scrapeJob);
        }
    }
}