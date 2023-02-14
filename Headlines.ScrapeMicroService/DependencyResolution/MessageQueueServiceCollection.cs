using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.MessageBroker;
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

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                    configurator.Host(new Uri(settings.Host), h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });

                    configurator.ReceiveEndpoint("article-scrape-service", x =>
                    {
                        x.Lazy = true;
                        x.PrefetchCount = 20;
                        x.Consumer<ArticleDetailScrapeRequestedEventConsumer>();
                    });
                });
            });

            services.AddTransient<IEventBus, EventBus>();

            return services;
        }
    }
}