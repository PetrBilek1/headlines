using AutoMapper;
using Headlines.BL.DAO;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.Enum;
using PBilek.ORM.Core.UnitOfWork;

namespace Headlines.BL.Facades
{
    public sealed class UserUpvotesFacade : IUserUpvotesFacade
    {
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly IUserUpvotesDao _userUpvotesDAO;
        private readonly IMapper _mapper;

        public UserUpvotesFacade(IUnitOfWorkProvider uowProvider, IUserUpvotesDao userUpvotesDAO, IMapper mapper)
        {
            _uowProvider = uowProvider;
            _userUpvotesDAO = userUpvotesDAO;
            _mapper = mapper;
        }

        public async Task<UserUpvotesDto> CreateOrUpdateUserUpvotesAsync(UserUpvotesDto upvotesDTO)
        {
            using IUnitOfWork uow = _uowProvider.CreateUnitOfWork();

            UserUpvotes upvotes = upvotesDTO.Id == default
                ? await _userUpvotesDAO.InsertAsync(new UserUpvotes())
                : await _userUpvotesDAO.AssertExistsAsync(upvotesDTO.Id);

            upvotes.UserToken = upvotesDTO.UserToken;
            upvotes.Json = upvotesDTO.Json;

            await uow.CommitAsync();

            return _mapper.Map<UserUpvotesDto>(upvotes);
        }

        public async Task<UserUpvotesDto> GetUserUpvotesByUserTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            using var _ = _uowProvider.CreateUnitOfWork(EntityTrackingOptions.NoTracking);

            UserUpvotes upvotes = await _userUpvotesDAO.GetByUserTokenAsync(token, cancellationToken);

            return _mapper.Map<UserUpvotesDto>(upvotes);
        }
    }
}