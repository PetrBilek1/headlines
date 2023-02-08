namespace Headlines.ScrapeMicroService
{
    public sealed class ServiceWorker : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(60);

        private readonly ILogger<ServiceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ServiceWorker(ILogger<ServiceWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scraping Hosted Service is starting.");

            await base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scraping Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}