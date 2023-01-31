using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1;
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
        private readonly MapperV1 _mapper;
        private readonly IArticleFacade _articleFacade;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public ArticlesController(IArticleFacade articleFacade)
        {
            _mapper = new MapperV1();
            _articleFacade = articleFacade;
        }

        [HttpPost("GetSkipTake")]
        public async Task<IActionResult> GetSkipTake([FromBody] GetSkipTakeRequest request, CancellationToken cancellationToken)
        {
            request.Skip ??= 0;
            request.Take ??= DefaultTake;
            request.Take = Math.Min(request.Take.Value, MaxTake);

            if (request.ArticleSources != null && !request.ArticleSources.Any())
            {
                return Ok(new GetSkipTakeResponse
                {
                    Articles = new(),
                    MatchesFiltersCount = 0
                });
            }

            List<ArticleDTO> articles = await _articleFacade.GetArticlesByFiltersSkipTakeAsync(request.Skip.Value, request.Take.Value, request.SearchPrompt, request.ArticleSources, cancellationToken);
            long count = await _articleFacade.GetArticlesCountByFiltersAsync(request.SearchPrompt, request.ArticleSources, cancellationToken);

            return Ok(new GetSkipTakeResponse
            {
                Articles = articles.Select(_mapper.MapArticle).ToList(),
                MatchesFiltersCount = count
            });
        }
    }
}