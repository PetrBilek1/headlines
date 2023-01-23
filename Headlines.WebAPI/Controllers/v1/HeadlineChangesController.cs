using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.WebAPI.Contracts.V1;
using Headlines.WebAPI.Contracts.V1.Models;
using Headlines.WebAPI.Contracts.V1.Requests.HeadlineChanges;
using Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges;
using Headlines.WebAPI.Resources.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBilek.Infrastructure.DatetimeProvider;

namespace Headlines.WebAPI.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public sealed class HeadlineChangesController : ControllerBase
    {
        private readonly MapperV1 _mapper;
        private readonly IHeadlineChangeFacade _headlineChangeFacade;
        private readonly IUserUpvotesFacade _userUpvotesFacade;
        private readonly IDateTimeProvider _dateTimeProvider;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public HeadlineChangesController(IHeadlineChangeFacade headlineChangeFacade, IUserUpvotesFacade userUpvotesFacade, IDateTimeProvider dateTimeProvider)
        {
            _mapper = new MapperV1();
            _headlineChangeFacade = headlineChangeFacade;
            _userUpvotesFacade = userUpvotesFacade;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("GetCount")]
        public async Task<IActionResult> GetCount()
        {
            long count = await _headlineChangeFacade.GetHeadlineChangeCountAsync();

            return Ok(count);
        }

        [HttpGet("GetTopUpvoted")]
        public async Task<IActionResult> GetTopUpvoted(CancellationToken cancellationToken, int take = DefaultTake)
        {
            take = Math.Min(take, MaxTake);

            List<HeadlineChangeDTO> headlineChanges = await _headlineChangeFacade.GetHeadlineChangesOrderByUpvotesCountIncludeArticleAsync(take, cancellationToken);

            return Ok(new GetTopUpvotedResponse
            {
                HeadlineChanges = headlineChanges.Select(_mapper.MapHeadlineChange).ToList()
            });
        }

        [HttpGet("GetSkipTake")]
        public async Task<IActionResult> GetSkipTake(int? skip, int? take, CancellationToken cancellationToken)
        {
            if (!skip.HasValue || !take.HasValue)
                return BadRequest(Messages.M0001);

            take = Math.Min(take.Value, MaxTake);

            List<HeadlineChangeDTO> headlineChanges = await _headlineChangeFacade.GetHeadlineChangesOrderByDetectedDescendingIncludeArticleAsync(skip.Value, take.Value, cancellationToken);

            return Ok(new GetSkipTakeResponse
            {
                HeadlineChanges = headlineChanges.Select(_mapper.MapHeadlineChange).ToList()
            });
        }

        [HttpPost("Upvote")]
        public async Task<IActionResult> Upvote([FromBody] UpvoteRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserToken))
                return BadRequest(Messages.M0002);

            UserUpvotesDTO userUpvotes = await _userUpvotesFacade.GetUserUpvotesByUserTokenAsync(request.UserToken, cancellationToken);
            userUpvotes ??= await _userUpvotesFacade.CreateOrUpdateUserUpvotesAsync(new UserUpvotesDTO
            {
                UserToken = request.UserToken,
                Json = JsonConvert.SerializeObject(new List<UpvoteModel>(), Formatting.None)
            });

            List<UpvoteModel> jsonUpvotes = JsonConvert.DeserializeObject<List<UpvoteModel>>(userUpvotes.Json) ?? new List<UpvoteModel>();

            if (jsonUpvotes.Any(x => x.Type == UpvoteType.HeadlineChange && x.TargetId == request.HeadlineChangeId))
                return Ok(new UpvoteResponse
                {
                    Upvotes = jsonUpvotes
                });

            HeadlineChangeDTO upvotedChange = await _headlineChangeFacade.AddUpvotesToHeadlineChangeAsync(request.HeadlineChangeId, 1);

            jsonUpvotes.Add(new UpvoteModel
            {
                Type = UpvoteType.HeadlineChange,
                Date = _dateTimeProvider.Now,
                TargetId = request.HeadlineChangeId,
            });

            userUpvotes.Json = JsonConvert.SerializeObject(jsonUpvotes, Formatting.None);

            UserUpvotesDTO updatedUserUpvotes = await _userUpvotesFacade.CreateOrUpdateUserUpvotesAsync(userUpvotes);

            return Ok(new UpvoteResponse 
            { 
                Upvotes = jsonUpvotes
            });
        }
    }
}