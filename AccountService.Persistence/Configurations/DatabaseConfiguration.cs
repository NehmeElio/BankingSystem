
//using AccountService.Persistence.Context;
//using AccountService.Persistence.Models;

using AccountService.Persistence.Context;
using AccountService.Persistence.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Persistence.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddDbContext<BankContext>();
        
        return services;
    }
}