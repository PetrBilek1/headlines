using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Abstractions.ObjectStorage;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1;
using Headlines.WebAPI.Contracts.V1.Requests.Articles;
using Headlines.WebAPI.Contracts.V1.Responses.Articles;
using Headlines.WebAPI.Extensions;
using Headlines.WebAPI.Resources.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

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
        private readonly IDistributedCache _cache;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public ArticlesController(IArticleFacade articleFacade, IHeadlineChangeFacade headlineChangeFacade, IObjectStorageWrapper objectStorage, IEventBus eventBus, IDistributedCache cache)
        {
            _mapper = new MapperV1();
            _articleFacade = articleFacade;
            _headlineChangeFacade = headlineChangeFacade;
            _objectStorage = objectStorage;
            _eventBus = eventBus;
            _cache = cache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            ArticleDto article = await _articleFacade.GetArticleByIdIncludeSourceAsync(id, cancellationToken);
            if (article == null)
                return NotFound();

            return Ok(new GetByIdResponse
            {
                Article = _mapper.MapArticle(article),
            });
        }

        [HttpGet("{id}/Detail")]
        public async Task<IActionResult> GetDetailById(long id, CancellationToken cancellationToken)
        {
            ArticleDto article = await _articleFacade.GetArticleByIdIncludeDetailsAsync(id, cancellationToken);
            if (article == null)
                return NotFound();

            ObjectDataDto? latestDetail = article.Details.OrderByDescending(x => x.Created).FirstOrDefault();
            if (latestDetail == null)
                return Ok(new GetDetailByIdResponse
                {
                    Detail = null
                });

            ArticleDetailDto? detail = await _cache.GetRecordAsync<ArticleDetailDto?>(GetObjectStorageCacheKey(latestDetail.Bucket, latestDetail.Key));
            if (detail == null)
            {
                detail = await _objectStorage.DownloadObjectAsync<ArticleDetailDto>(latestDetail.Bucket, latestDetail.Key, cancellationToken);
                await _cache.SetRecordAsync(GetObjectStorageCacheKey(latestDetail.Bucket, latestDetail.Key), detail, TimeSpan.FromMinutes(5));
            }

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
                return Ok(new SearchResponse
                {
                    Articles = new(),
                    MatchesFiltersCount = 0
                });

            List<ArticleDto> articles = await _articleFacade.GetArticlesByFiltersSkipTakeAsync(request.Skip.Value, request.Take.Value, request.SearchPrompt, request.ArticleSources, request.PublishedUtcFrom, request.PublishedUtcTo, cancellationToken);
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
            ArticleDto article = await _articleFacade.GetArticleByIdIncludeSourceAsync(request.ArticleId, cancellationToken);
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
        public async Task<IActionResult> GetHeadlineChangesByArticleId(long id, int skip, int take, CancellationToken cancellationToken)
        {
            take = Math.Min(take, MaxTake);

            List<HeadlineChangeDto> headlineChanges = await _headlineChangeFacade.GetHeadlineChangesByArticleIdOrderByDetectedDescendingAsync(id, skip, take, cancellationToken);
            long count = await _headlineChangeFacade.GetHeadlineChangeCountAsync(id);

            return Ok(new GetHeadlineChangesByArticleIdResponse
            {
                HeadlineChanges = headlineChanges.Select(_mapper.MapHeadlineChange).ToList(),
                TotalCount = count
            });
        }

        private static string GetObjectStorageCacheKey(string bucket, string key) => $"ObjectData-Bucket:{bucket}-Key:{key}";
    }
}