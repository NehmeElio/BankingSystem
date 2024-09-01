

using Microsoft.Extensions.DependencyInjection;
using TransactionService.Persistence.Context;

namespace TransactionService.Persistence.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddDbContext<BankContext>();
        
        return services;
    }
}