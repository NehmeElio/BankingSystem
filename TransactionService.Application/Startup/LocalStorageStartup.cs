
using Microsoft.Extensions.Hosting;
using TransactionService.Application.Caching;

namespace TransactionService.Application.Startup;

public class LocalStorageStartup(ILocalStorage localStorage) : Microsoft.Extensions.Hosting.BackgroundService, IStartup
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await localStorage.LoadEntitiesAsync(stoppingToken);
    }
}