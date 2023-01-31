using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Microsoft.AspNetCore.Mvc;

namespace Headlines.WebAPI.Controllers.v1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public sealed class ArticlesController : ControllerBase
    {
        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        [HttpPost("GetArticlesSkipTake")]
        public async Task<IActionResult> GetArticlesSkipTake([FromBody] GetArticlesSkipTakeRequest request, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}