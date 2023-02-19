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
        private const int RetryCount = 1;

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
                    await ScrapeUnsuccessfulAsync(context, article);
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
            catch (ArgumentNullException)
            {
                _logger.LogInformation("There was attempt to scrape article with Id '{articleId}' which's source has no Scraper assigned.", context.Message.ArticleId);
            }
        }

        private async Task ScrapeUnsuccessfulAsync(ConsumeContext<ArticleDetailScrapeRequestedEvent> context, ArticleDTO article)
        {
            _logger.LogWarning("Scraping of article Id '{articleId} source Name '{sourceName}' was not successful.'", article.Id, article.Source.Name);

            if (context.Message.Retried >= RetryCount)
            {
                _logger.LogError("Scraping of article with Id '{articleId}' was not successful '{retries}'.", context.Message.ArticleId, RetryCount);
                throw new ConsumerException($"Scraping of article with Id '{context.Message.ArticleId}' source Name '{article.Source.Name}' was not successful '{context.Message.Retried + 1}' times.");
            }

            await context.SchedulePublish(TimeSpan.FromSeconds(5 * context.Message.Retried), new ArticleDetailScrapeRequestedEvent
            {
                ArticleId = context.Message.ArticleId,
                Retried = context.Message.Retried + 1,
            });
        }
    }
}