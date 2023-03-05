using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Implementations.MessageBroker;
using Headlines.ScrapeMicroService.Consumers;
using MassTransit;

namespace Headlines.ScrapeMicroService.DependencyResolution
{
    public static class MessageQueueServiceCollection
    {
        public static IServiceCollection AddMessageQueueDependencyGroup(this IServiceCollection services, MessageBrokerSettings messageBrokerSettings)
        {
            services.AddSingleton(messageBrokerSettings);

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddDelayedMessageScheduler();
                busConfigurator.AddConsumer<ArticleDetailScrapeRequestedEventConsumer>();
                busConfigurator.AddConsumer<ArticleDetailUploadRequestedEventConsumer>();

                if (string.IsNullOrEmpty(messageBrokerSettings.Host))
                {
                    busConfigurator.UsingInMemory();
                    return;
                }

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                    configurator.Host(new Uri(settings.Host), h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });

                    configurator.UseDelayedMessageScheduler();

                    configurator.ReceiveEndpoint("scrape-article-detail-service", x =>
                    {
                        x.Lazy = true;
                        x.PrefetchCount = 20;
                        x.Consumer<ArticleDetailScrapeRequestedEventConsumer>(context);
                    });

                    configurator.ReceiveEndpoint("upload-article-detail-service", x =>
                    {
                        x.Lazy = true;
                        x.PrefetchCount = 20;
                        x.Consumer<ArticleDetailUploadRequestedEventConsumer>(context);
                    });
                });
            });

            services.AddTransient<IEventBus, EventBus>();

            return services;
        }
    }
}