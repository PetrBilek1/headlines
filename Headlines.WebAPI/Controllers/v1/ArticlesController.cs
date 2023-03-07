using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Resources.V1;
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
        private readonly IHeadlineChangeFacade _headlineChangeFacade;
        private readonly IObjectStorageWrapper _objectStorage;
        private readonly IEventBus _eventBus;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public ArticlesController(IArticleFacade articleFacade, IHeadlineChangeFacade headlineChangeFacade, IObjectStorageWrapper objectStorage, IEventBus eventBus)
        {
            _mapper = new MapperV1();
            _articleFacade = articleFacade;
            _headlineChangeFacade = headlineChangeFacade;
            _objectStorage = objectStorage;
            _eventBus = eventBus;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return BadRequest(Messages.M0003);

            ArticleDTO article = await _articleFacade.GetArticleByIdIncludeSourceAsync(id.Value, cancellationToken);
            if (article == null)
                return NotFound();

            return Ok(new GetByIdResponse
            {
                Article = _mapper.MapArticle(article),
            });
        }

        [HttpGet("{id}/Detail")]
        public async Task<IActionResult> GetDetailById(long? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return BadRequest(Messages.M0003);

            ArticleDTO article = await _articleFacade.GetArticleByIdIncludeDetailsAsync(id.Value, cancellationToken);
            if (article == null)
                return NotFound();

            ObjectDataDTO? latestDetail = article.Details.OrderByDescending(x => x.Created).FirstOrDefault();
            if (latestDetail == null)
                return Ok(new GetDetailByIdResponse
                {
                    Detail = null
                });

            ArticleDetailDTO? detail = await _objectStorage.DownloadObjectAsync<ArticleDetailDTO>(latestDetail.Bucket, latestDetail.Key, cancellationToken);

            return Ok(new GetDetailByIdResponse
            {
                Detail = detail == null ? null : _mapper.MapArticleDetail(detail)
            });
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            request.Skip ??= 0;
            request.Take ??= DefaultTake;
            request.Take = Math.Min(request.Take.Value, MaxTake);

            if (request.ArticleSources != null && !request.ArticleSources.Any())
            {
                return Ok(new SearchResponse
                {
                    Articles = new(),
                    MatchesFiltersCount = 0
                });
            }

            List<ArticleDTO> articles = await _articleFacade.GetArticlesByFiltersSkipTakeAsync(request.Skip.Value, request.Take.Value, request.SearchPrompt, request.ArticleSources, null, null, cancellationToken);
            long count = await _articleFacade.GetArticlesCountByFiltersAsync(request.SearchPrompt, request.ArticleSources, null, null, cancellationToken);

            return Ok(new SearchResponse
            {
                Articles = articles.Select(_mapper.MapArticle).ToList(),
                MatchesFiltersCount = count
            });
        }

        [HttpPost("RequestDetailScrape")]
        public async Task<IActionResult> RequestDetailScrape([FromBody] RequestDetailScrapeRequest request, CancellationToken cancellationToken)
        {
            ArticleDTO article = await _articleFacade.GetArticleByIdIncludeSourceAsync(request.ArticleId);
            if (article == null)
                return NotFound(Messages.M0004);
            if (!article.Source.ScraperType.HasValue)
                return BadRequest(Messages.M0005);

            await _eventBus.PublishAsync(new ArticleDetailScrapeRequestedEvent
            {
                ArticleId = article.Id,
            },
            cancellationToken);

            return Ok();
        }

        [HttpGet("{id}/HeadlineChanges/Skip/{skip}/Take/{take}")]
        public async Task<IActionResult> GetHeadlineChangesByArticleId(long? id, int? skip, int? take, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return BadRequest(Messages.M0003);

            if (!skip.HasValue || !take.HasValue)
                return BadRequest(Messages.M0001);

            take = Math.Min(take.Value, MaxTake);

            List<HeadlineChangeDTO> headlineChanges = await _headlineChangeFacade.GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(id.Value, skip.Value, take.Value, cancellationToken);
            long count = await _headlineChangeFacade.GetHeadlineChangeCountAsync(id);

            return Ok(new GetHeadlineChangesByArticleIdResponse
            {
                HeadlineChanges = headlineChanges.Select(_mapper.MapHeadlineChange).ToList(),
                TotalCount = count
            });
        }
    }
}