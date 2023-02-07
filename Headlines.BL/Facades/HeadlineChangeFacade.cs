using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class HeadlineChangeFacade : IHeadlineChangeFacade
    {
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IHeadlineChangeDAO _headlineChangeDAO;
        private readonly IMapper _mapper;

        public HeadlineChangeFacade(IUnitOfWorkProvider uowProvider, IHeadlineChangeDAO headlineChangeDAO, IMapper mapper)
        {
            _uowProvider = uowProvider;
            _headlineChangeDAO = headlineChangeDAO;
            _mapper = mapper;
        }

        public async Task<HeadlineChangeDTO> CreateOrUpdateHeadlineChangeAsync(HeadlineChangeDTO headlineChangeDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            HeadlineChange headlineChange = headlineChangeDTO.Id == default
                ? await _headlineChangeDAO.InsertAsync(new HeadlineChange())
                : await _headlineChangeDAO.AssertExistsAsync(headlineChangeDTO.Id);

            headlineChange.ArticleId = headlineChangeDTO.ArticleId;
            headlineChange.Detected = headlineChangeDTO.Detected;
            headlineChange.TitleBefore = headlineChangeDTO.TitleBefore;
            headlineChange.TitleAfter = headlineChangeDTO.TitleAfter;
            headlineChange.UpvoteCount = headlineChangeDTO.UpvoteCount;

            await uow.CommitAsync();

            return _mapper.Map<HeadlineChangeDTO>(headlineChange);
        }

        public async Task<HeadlineChangeDTO> DeleteHeadlineChangeAsync(HeadlineChangeDTO headlineChangeDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();
            
            HeadlineChange headlineChange = await _headlineChangeDAO.GetByIdAsync(headlineChangeDTO.Id);

            if (headlineChange == null)
                throw new Exception($"HeadlineChange with Id '{headlineChangeDTO.Id}' does not exist.");

            _headlineChangeDAO.Delete(headlineChange);

            await uow.CommitAsync();

            return _mapper.Map<HeadlineChangeDTO>(headlineChange);
        }

        public async Task<List<HeadlineChangeDTO>> GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<HeadlineChange> changes = await _headlineChangeDAO.GetOrderByUpvotesCountIncludeArticleAsync(take, cancellationToken);

            return _mapper.Map<List<HeadlineChangeDTO>>(changes);
        }

        public async Task<List<HeadlineChangeDTO>> GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<HeadlineChange> changes = await _headlineChangeDAO.GetOrderByDetectedDescendingIncludeArticleAsync(skip, take, cancellationToken);

            return _mapper.Map<List<HeadlineChangeDTO>>(changes);
        }

        public async Task<long> GetHeadlineChangeCountAsync(long? articleId = null)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            long count = await _headlineChangeDAO.GetCountAsync(articleId);

            return count;
        }

        public async Task<List<HeadlineChangeDTO>> GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(long articleId, int skip, int take, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            List<HeadlineChange> changes = await _headlineChangeDAO.GetByArticleIdOrderByDetectedDescendingAsync(articleId, skip, take, cancellationToken);

            return _mapper.Map<List<HeadlineChangeDTO>>(changes);
        }

        public async Task<HeadlineChangeDTO> AddUpvotesToHeadlineChangeAsync(long id, int amount)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            HeadlineChange headlineChange = await _headlineChangeDAO.GetByIdAsync(id);

            if (headlineChange == null)
                throw new Exception($"HeadlineChange with Id '{id}' not found.");

            headlineChange.UpvoteCount += amount;

            await uow.CommitAsync();

            return _mapper.Map<HeadlineChangeDTO>(headlineChange);
        }
    }
}