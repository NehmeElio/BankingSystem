
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Factory;
using TransactionService.Application.Interfaces;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Services;

public class RecurrentTransactionService:IRecurrentTransactionService
{
    private readonly BankContext _dbContext;
    private readonly IBankContextFactory _dbContextFactory;
    private readonly ILogger<RecurrentTransactionService> _logger;

    public RecurrentTransactionService(BankContext dbContext, IBankContextFactory dbContextFactory, ILogger<RecurrentTransactionService> logger)
    {
        _dbContext = dbContext;//with default postgres user
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }
    public async Task ProcessRecurrentTransactionsAsync(CancellationToken cancellationToken)
    {
        //we have to process recurrent transactions for each branch so we need to get all branches
        var branches=await _dbContext.Branches.ToListAsync(cancellationToken: cancellationToken);
        
        foreach (var branch in branches)
        {
            if (branch.BranchName != null) await ProcessSchemaAsync(branch.BranchName,cancellationToken);
        }
    }

    private async Task ProcessSchemaAsync(string schema, CancellationToken cancellationToken)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext(schema);

        var transactions = await dbContext.RecurrentTransactions.ToListAsync(cancellationToken: cancellationToken);
    
        foreach (var transaction in transactions)
        {
            var nextExpectedDate = transaction.NextExecutionDate;

            if (nextExpectedDate > DateOnly.FromDateTime(DateTime.Now)) continue;

            // Update LastExecutionDate
            transaction.LastExecutionDate = transaction.NextExecutionDate;

            // Fetch IntervalType
            var intervalTypeId = transaction.IntervalTypeId;
            var intervalType = await dbContext.IntervalTypes.FindAsync(intervalTypeId);

            if (intervalType != null)
            {
                // Adjust NextExecutionDate based on IntervalType
                switch (intervalType.IntervalTypeName)
                {
                    case "Daily":
                        transaction.NextExecutionDate = transaction.LastExecutionDate?.AddDays(1);
                        break;
                    case "Weekly":
                        transaction.NextExecutionDate = transaction.LastExecutionDate?.AddDays(7);
                        break;
                    case "Monthly":
                        transaction.NextExecutionDate = transaction.LastExecutionDate?.AddMonths(1);
                        break;
                    default:
                        _logger.LogWarning($"Unknown interval type: {intervalType.IntervalTypeName}");
                        break;
                }

                transaction.Version += 1;

                // Update account balance
                var accountId = transaction.AccountId;
                var account = await dbContext.Accounts.FindAsync(accountId);
                if (account != null)
                {
                    account.Balance += transaction.IntervalValue;
                }

                // Save changes
                dbContext.RecurrentTransactions.Update(transaction);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

}