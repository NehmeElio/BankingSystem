using AccountService.Application.Caching;
using AccountService.Application.Startup;
using Microsoft.Extensions.DependencyInjection;
using UMS_Lab5.Caching;

namespace AccountService.Application.Configurations;

public static class CachingConfiguration
{
    public static void AddCaching(this IServiceCollection services)
    {
        services.AddCachingServices();
        services.AddSingleton<ILocalStorage,LocalStorage >();
        services.AddHostedService<LocalStorageStartup>();
    }
}