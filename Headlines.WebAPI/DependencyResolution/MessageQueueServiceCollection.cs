using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Implementations.MessageBroker;
using Headlines.WebAPI.Consumers;
using MassTransit;

namespace Headlines.WebAPI.DependencyResolution
{
    public static class MessageQueueServiceCollection
    {
        public static IServiceCollection AddMessageQueueDependencyGroup(this IServiceCollection services, MessageBrokerSettings messageBrokerSettings)
        {
            services.AddSingleton(messageBrokerSettings);

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<ArticleDetailScrapeResultEventConsumer>();

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

                    configurator.ReceiveEndpoint($"websocket-article-detail-service-{messageBrokerSettings.ReplicaName}", x =>
                    {
                        x.Lazy = true;
                        x.PrefetchCount = 20;
                        x.Consumer<ArticleDetailScrapeResultEventConsumer>(context);
                    });
                });
            });

            services.AddTransient<IEventBus, EventBus>();

            return services;
        }
    }
}