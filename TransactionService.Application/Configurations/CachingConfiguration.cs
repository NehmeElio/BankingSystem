
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Application.Caching;
using TransactionService.Application.Startup;
using UMS_Lab5.Caching;

namespace TransactionService.Application.Configurations;

public static class CachingConfiguration
{
    public static void AddCaching(this IServiceCollection services)
    {
        services.AddCachingServices();
        services.AddSingleton<ILocalStorage,LocalStorage >();
        services.AddHostedService<LocalStorageStartup>();
    }
}