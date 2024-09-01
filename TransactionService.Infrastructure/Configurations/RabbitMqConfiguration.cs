
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Infrastructure.Services;
using TransactionService.Persistence.Interfaces;
using TransactionService.Persistence.Services;

namespace TransactionService.Infrastructure.Configurations;

public static class RabbitMqConfiguration
{
    public static void AddRabbitMqService<T>(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<RabbitMqReceiverService<T>>();
    }
}