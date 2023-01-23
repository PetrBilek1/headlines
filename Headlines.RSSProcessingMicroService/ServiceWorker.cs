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

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RSS Reader Hosted Service is starting.");

            await base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_readingPeriod);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using IServiceScope scope = _serviceProvider.CreateScope();

                    IRSSProcessorService processorService = scope.ServiceProvider.GetRequiredService<IRSSProcessorService>();

                    await processorService.DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to execute RSSProcessorService with exception message {ex.Message}.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RSS Reader Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}