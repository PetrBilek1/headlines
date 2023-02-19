using Headlines.BL.Abstractions.EventBus;
using Headlines.BL.Implementations.MessageBroker;
using MassTransit;

namespace Headlines.RSSProcessingMicroService.DependencyResolution
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

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                    configurator.Host(new Uri(settings.Host), h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });

                    configurator.UseDelayedMessageScheduler();
                });
            });

            services.AddTransient<IEventBus, EventBus>();

            return services;
        }
    }
}