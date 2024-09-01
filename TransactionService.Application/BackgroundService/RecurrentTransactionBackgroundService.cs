using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Interfaces;

namespace TransactionService.Application.BackgroundService;

public class RecurrentTransactionBackgroundService:Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger<RecurrentTransactionBackgroundService> _logger;
    //private readonly IRecurrentTransactionService _transactionService;
    private readonly TimeSpan _interval = TimeSpan.FromDays(1);//so we run it 
    private readonly IServiceProvider _serviceProvider;

    public RecurrentTransactionBackgroundService(ILogger<RecurrentTransactionBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        //_transactionService = transactionService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Calculate initial delay until the next 8 AM
        var now = DateTime.Now;
        var nextRun = DateTime.Today.AddHours(19).AddMinutes(3);
        if (now > nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        var initialDelay = nextRun - now;

        // Wait for the initial delay before starting the loop
        await Task.Delay(initialDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Processing recurrent transactions at: {time}", DateTimeOffset.Now);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transactionService = scope.ServiceProvider.GetRequiredService<IRecurrentTransactionService>();
                    await transactionService.ProcessRecurrentTransactionsAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing recurrent transactions.");
            }

            //wait for the specified interval before running the task again
            await Task.Delay(_interval, stoppingToken);
        }
    }
    
}