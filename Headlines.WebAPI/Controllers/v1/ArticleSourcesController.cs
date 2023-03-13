using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1;
using Headlines.WebAPI.Contracts.V1.Responses.ArticleSources;
using Microsoft.AspNetCore.Mvc;

namespace Headlines.WebAPI.Controllers.v1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public sealed class ArticleSourcesController : ControllerBase
    {
        private readonly MapperV1 _mapper;
        private readonly IArticleSourceFacade _articleSourceFacade;

        public ArticleSourcesController(IArticleSourceFacade articleSourceFacade)
        {
            _mapper = new MapperV1();
            _articleSourceFacade = articleSourceFacade;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            List<ArticleSourceDto> sources = await _articleSourceFacade.GetAllArticleSourcesAsync(cancellationToken);

            return Ok(new GetAllResponse
            {
                ArticleSources = sources.Select(_mapper.MapArticleSource).ToList()
            });
        }
    }
}