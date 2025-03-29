using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using Rmq.Consumer.Api.Consumers;
using Rmq.Consumer.Api.Settings;

namespace Rmq.Consumer.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
        services.AddSingleton(rabbitMqSettings);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<JsonRpcConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    rabbitMqSettings.Host, 
                    rabbitMqSettings.VirtualHost, 
                    h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });
                    
                cfg.ConfigureJsonSerializerOptions(options => 
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.Converters.Add(new JsonStringEnumConverter());
                    return options;
                });
                
                cfg.ReceiveEndpoint("jsonrpc-queue", e =>
                {
                    e.ConfigureConsumer<JsonRpcConsumer>(context);
                });
                    
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

