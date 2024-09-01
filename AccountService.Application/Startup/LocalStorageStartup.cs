using AccountService.Application.Caching;
using Microsoft.Extensions.Hosting;
using UMS_Lab5.Application.Startup;

namespace AccountService.Application.Startup;

public class LocalStorageStartup(ILocalStorage localStorage) : BackgroundService, IStartup
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await localStorage.LoadEntitiesAsync(stoppingToken);
    }
}