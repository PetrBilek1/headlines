using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Events;
using Headlines.BL.Facades;
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
            _logger.LogInformation("Received request to scrape article with Id '{articleId}'", context.Message.ArticleId);

            try
            {
                ArticleDTO article = await _articleFacade.GetArticleByIdIncludeSourceAsync(context.Message.ArticleId);
            }
            finally
            {

            }
        }
    }
}