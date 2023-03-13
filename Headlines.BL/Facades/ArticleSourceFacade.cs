using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class ArticleSourceFacade : IArticleSourceFacade
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IArticleSourceDao _articleSourceDAO;

        public ArticleSourceFacade(IMapper mapper, IUnitOfWorkProvider uowProvider, IArticleSourceDao articleSourceDAO)
        {
            _mapper = mapper;
            _uowProvider = uowProvider;
            _articleSourceDAO = articleSourceDAO;
         }

        public async Task<List<ArticleSourceDto>> GetAllArticleSourcesAsync(CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<ArticleSource> sources = await _articleSourceDAO.GetAllAsync(cancellationToken);

            return _mapper.Map<List<ArticleSourceDto>>(sources);
        }
    }
}