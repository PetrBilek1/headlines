using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using MassTransit;

namespace Headlines.ScrapeMicroService.Consumers
{
    public sealed class ArticleDetailScrapeRequestedEventConsumer : IConsumer<ArticleDetailScrapeRequestedEvent>
    {
        private readonly IArticleFacade _articleFacade;
        private readonly IArticleScraperProvider _scraperProvider;
        private readonly ILogger<ArticleDetailScrapeRequestedEventConsumer> _logger;

        public ArticleDetailScrapeRequestedEventConsumer(IArticleFacade articleFacade, IArticleScraperProvider scraperProvider, ILogger<ArticleDetailScrapeRequestedEventConsumer> logger)
        {
            _articleFacade = articleFacade;
            _scraperProvider = scraperProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ArticleDetailScrapeRequestedEvent> context)
        {
            _logger.LogInformation("Received request to scrape article with Id '{articleId}'.", context.Message.ArticleId);

            try
            {
                ArticleDTO article = await _articleFacade.GetArticleByIdIncludeSourceAsync(context.Message.ArticleId);
                ArgumentNullException.ThrowIfNull(article.Source.ScraperType);

                IArticleScraper scraper = _scraperProvider.Provide(article.Source.ScraperType.Value);
                ArticleScrapeResult result = await scraper.ScrapeArticleAsync(article.Link);                

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Scraping of article Id '{articleId} was not successful.'", article.Id);
                    return;
                }

                await context.Publish(new ArticleDetailUploadRequestedEvent
                {
                    ArticleId = context.Message.ArticleId,
                    Detail = new ArticleDetailDTO
                    {
                        IsPaywalled = result.IsPaywalled,
                        Title = result.Title,
                        Author = result.Author,
                        Paragraphs = result.Paragraphs,
                        Tags = result.Tags,
                    }
                });
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There was and exception when trying to scrape article with Id '{articleId}'.", context.Message.ArticleId);
            }
        }
    }
}