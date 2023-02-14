using Headlines.BL.Events;
using MassTransit;

namespace Headlines.ScrapeMicroService.Consumers
{
    public sealed class ArticleDetailScrapeRequestedEventConsumer : IConsumer<ArticleDetailScrapeRequestedEvent>
    {
        public Task Consume(ConsumeContext<ArticleDetailScrapeRequestedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}