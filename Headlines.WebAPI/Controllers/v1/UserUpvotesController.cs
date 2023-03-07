using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Responses.UserUpvotes;
using Headlines.WebAPI.Resources.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserUpvotesController : ControllerBase
    {
        private readonly IUserUpvotesFacade _userUpvotesFacade;

        public UserUpvotesController(IUserUpvotesFacade userUpvotesFacade)
        {
            _userUpvotesFacade = userUpvotesFacade;
        }

        [HttpGet("{userToken}")]
        public async Task<IActionResult> Get(string userToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userToken))
                return BadRequest(Messages.M0002);

            UserUpvotesDTO userUpvotes = await _userUpvotesFacade.GetUserUpvotesByUserTokenAsync(userToken, cancellationToken);

            userUpvotes ??= await _userUpvotesFacade.CreateOrUpdateUserUpvotesAsync(new UserUpvotesDTO
            {
                UserToken = userToken,
                Json = JsonConvert.SerializeObject(new List<UpvoteModel>(), Formatting.None)
            });

            return Ok(new GetResponse
            {
                Upvotes = JsonConvert.DeserializeObject<List<UpvoteModel>>(userUpvotes.Json)!
            });
        }       
    }
}