﻿using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Events;
using Headlines.BL.Facades;
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
        private readonly IEventBus _eventBus;

        public const int MaxTake = 50;
        public const int DefaultTake = 10;

        public ArticlesController(IArticleFacade articleFacade, IEventBus eventBus)
        {
            _mapper = new MapperV1();
            _articleFacade = articleFacade;
            _eventBus = eventBus;
        }

        [HttpGet("GetById")]
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

            List<ArticleDTO> articles = await _articleFacade.GetArticlesByFiltersSkipTakeAsync(request.Skip.Value, request.Take.Value, request.SearchPrompt, request.ArticleSources, null, null, cancellationToken);
            long count = await _articleFacade.GetArticlesCountByFiltersAsync(request.SearchPrompt, request.ArticleSources, null, null, cancellationToken);

            return Ok(new GetSkipTakeResponse
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

            await _eventBus.PublishAsync(new ArticleDetailScrapeRequestedEvent
            {
                ArticleId = article.Id,
            },
            cancellationToken);

            return Ok();
        }
    }
}