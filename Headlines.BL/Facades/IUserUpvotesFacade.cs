using Headlines.DTO.Entities;

namespace Headlines.BL.Facades
{
    public interface IUserUpvotesFacade
    {
        Task<UserUpvotesDTO> CreateOrUpdateUserUpvotesAsync(UserUpvotesDTO upvotesDTO);
        Task<UserUpvotesDTO> GetUserUpvotesByUserTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}