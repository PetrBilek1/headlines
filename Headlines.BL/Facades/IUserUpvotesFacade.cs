using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IUserUpvotesFacade
    {
        Task<UserUpvotesDto> CreateOrUpdateUserUpvotesAsync(UserUpvotesDto upvotesDTO);
        Task<UserUpvotesDto> GetUserUpvotesByUserTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}