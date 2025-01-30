using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extentions
    {
        public static IServiceCollection AddMessageBroker
            (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {
            // Add MassTransit to the services collection and configure it to use RabbitMQ as the message broker and add the consumers from the assembly
            services.AddMassTransit(config =>
            {
                // Set naming covention to endpoint name formatter to kebab case
                config.SetKebabCaseEndpointNameFormatter(); 

                if (assembly != null)
                    config.AddConsumers(assembly);

                // Add RabbitMQ as the message broker and configure the endpoints using the configuration from the appsettings.json
                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]);
                        host.Password(configuration["MessageBroker:Password"]);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
