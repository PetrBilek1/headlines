using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Events;
using Headlines.DTO.Entities;
using Headlines.RSSProcessingMicroService.Services;

namespace Headlines.RSSProcessingMicroService
{
    public sealed class ServiceWorker : BackgroundService
    {
        private readonly TimeSpan _readingPeriod = TimeSpan.FromSeconds(60);

        private readonly ILogger<ServiceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ServiceWorker(ILogger<ServiceWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RSS Reader Hosted Service is starting.");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_readingPeriod);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using IServiceScope scope = _serviceProvider.CreateScope();

                    IRssProcessorService processorService = scope.ServiceProvider.GetRequiredService<IRssProcessorService>();

                    var result = await processorService.DoWorkAsync(stoppingToken);

                    await PublishScrapeRequestsAsync(result.CreatedArticles, scope);                    
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to execute RSSProcessorService with exception message '{message}'.", ex.Message);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RSS Reader Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }

        private static async Task PublishScrapeRequestsAsync(List<ArticleDto> articles, IServiceScope scope)
        {
            IEventBus eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var random = new Random();
            var shuffledArticles = articles.OrderBy(x => random.Next()).ToList();

            foreach (var article in shuffledArticles)
            {
                await eventBus.PublishAsync(new ArticleDetailScrapeRequestedEvent
                {
                    ArticleId = article.Id,
                });
            }
        }
    }
}