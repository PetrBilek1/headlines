using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Microsoft.AspNetCore.Mvc;

namespace Headlines.WebAPI.Controllers.v1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public sealed class ArticlesController : ControllerBase
    {
        private readonly IArticleSourceFacade _articleSourceFacade;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public ArticlesController(IArticleSourceFacade articleSourceFacade)
        {
            _articleSourceFacade = articleSourceFacade;
        }

        [HttpPost("GetSkipTake")]
        public async Task<IActionResult> GetSkipTake([FromBody] GetSkipTakeRequest request, CancellationToken cancellationToken)
        {
            request.Skip ??= 0;
            request.Take ??= DefaultTake;
            request.Take = Math.Min(request.Take.Value, MaxTake);

            if (request.ArticleSources == null)
            {
                List<ArticleSourceDTO> sources = await _articleSourceFacade.GetAllArticleSourcesAsync(cancellationToken);
                request.ArticleSources = sources.Select(x => x.Id).ToArray();
            }
            if (!request.ArticleSources.Any())
            {
                return Ok(new GetSkipTakeResponse
                {
                    Articles = new(),
                    MatchesFiltersCount = 0
                });
            }               



            return Ok(new GetSkipTakeResponse
            {
                Articles = new(),
                MatchesFiltersCount = 0
            });
        }
    }
}