
using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Caching;
using TransactionService.Application.Commands;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class RollbackHandler:IRequestHandler<RollbackCommand>
{
    private readonly IBankContextFactory _bankContextFactory;
    private readonly BankContext _bankContext;
    private readonly ILogger<RollbackHandler> _logger;
    private readonly ILocalStorage _localStorage;

    public RollbackHandler(IBankContextFactory bankContextFactory, BankContext bankContext, ILogger<RollbackHandler> logger, ILocalStorage localStorage)
    {
        _bankContextFactory = bankContextFactory;
        _bankContext = bankContext;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task Handle(RollbackCommand request, CancellationToken cancellationToken)
    {
        if(request.RollbackDto.RollbackDate>DateOnly.FromDateTime(DateTime.Now)) throw new InvalidDateException("Rollback date cannot be in the future");
        if(request.RollbackDto.Username!=null) 
            await ProcessUserAsync(request.RollbackDto.Username,request.RollbackDto.RollbackDate,cancellationToken);
        else await ProcessDatabaseAsync(request.RollbackDto.RollbackDate,cancellationToken);
    }

    private async Task ProcessDatabaseAsync(DateOnly rollbackDate, CancellationToken cancellationToken)
    {
        var branches = await _bankContext.Branches.ToListAsync(cancellationToken: cancellationToken);
        foreach (Branch branch in branches)
        {
            if (branch.BranchName != null)
            {
                await using (var dbcontext = _bankContextFactory.CreateDbContext(branch.BranchName))
                {
                    var transactions = await dbcontext.Transactions.ToListAsync(cancellationToken);
                    foreach (Transaction transaction in transactions)
                    {
                        await ProcessTransactionAsync(rollbackDate, transaction, dbcontext, cancellationToken);
                    }

                    var recurrentTransactions = await dbcontext.RecurrentTransactions.ToListAsync(cancellationToken);
                    foreach (RecurrentTransaction recurrentTransaction in recurrentTransactions)
                    {
                        await ProcessRecurrentTransactionAsync(rollbackDate, recurrentTransaction, dbcontext, cancellationToken);
                    }

                    await dbcontext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }


    private async Task ProcessUserAsync(string username, DateOnly rollbackDate, CancellationToken cancellationToken)
    {
        var userExists=await _bankContext.Users.AnyAsync(u=>u.Username==username,cancellationToken: cancellationToken);
        if(!userExists) throw new NotFoundException("User not found");
        
        var branchId=await _bankContext.Users.Where(u=>u.Username==username).Select(u=>u.BranchId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var branchName=await _bankContext.Branches.Where(b=>b.BranchId==branchId).Select(b=>b.BranchName).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (branchName != null)
        {
            await using (var dbcontext = _bankContextFactory.CreateDbContext(branchName))
            {
                var transactions = await dbcontext.Transactions.ToListAsync(cancellationToken);

                foreach (Transaction transaction in transactions)
                {
                    await ProcessTransactionAsync(rollbackDate, transaction, dbcontext, cancellationToken);
                }
                var recurrentTransactions = await dbcontext.RecurrentTransactions.ToListAsync(cancellationToken);

                foreach (RecurrentTransaction recurrentTransaction in recurrentTransactions)
                {
                    await ProcessRecurrentTransactionAsync(rollbackDate, recurrentTransaction, dbcontext,
                        cancellationToken);
                }

                await dbcontext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private  async Task ProcessTransactionAsync(DateOnly rollbackDate,Transaction transaction, BankContext bankContext,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Transaction has intervaltype id {transaction.TransactionId}");
        if(transaction.TransactionDate<rollbackDate) return;

        if(transaction.TransactionTypeId==_localStorage.TransactionTypes["Deposit"])
        {
            var account =await bankContext.Accounts.FindAsync(transaction.AccountId,cancellationToken);
            if (account != null) account.Balance -= transaction.Amount;
            bankContext.Transactions.Remove(transaction);
        }
        else if(transaction.TransactionTypeId==_localStorage.TransactionTypes["Withdrawal"])
        {
            var account =await bankContext.Accounts.FindAsync(transaction.AccountId,cancellationToken);
            if (account != null) account.Balance += transaction.Amount;
            bankContext.Transactions.Remove(transaction);
        }
        else
        {
            throw new NotFoundException("Transaction Type not found");
        }
    }
    
    private async Task ProcessRecurrentTransactionAsync(DateOnly rollbackDate,RecurrentTransaction recurrentTransaction, BankContext bankContext,
        CancellationToken cancellationToken)
    {
        if(recurrentTransaction.CreatedDate<rollbackDate) return;
        

        var totalAmountOfPaymentMade = recurrentTransaction.Version??0;
        var amountOfPaymentToRollback=CalculateAmountOfPaymentToRollback(recurrentTransaction,rollbackDate);
        
        var account =await bankContext.Accounts.FindAsync(new object?[] { recurrentTransaction.AccountId, cancellationToken }, cancellationToken: cancellationToken);
        if (account != null)
        {
            account.Balance -= recurrentTransaction.IntervalValue * amountOfPaymentToRollback;
            recurrentTransaction.Version = (totalAmountOfPaymentMade - amountOfPaymentToRollback)<0?0:totalAmountOfPaymentMade - amountOfPaymentToRollback;
            if(recurrentTransaction.Version==0) bankContext.RecurrentTransactions.Remove(recurrentTransaction);

        }

        await bankContext.SaveChangesAsync(cancellationToken);
    }

    private int CalculateAmountOfPaymentToRollback(RecurrentTransaction transaction, DateOnly rollbackDate)
    {
        var recurrentTransactionLastExecutionDate = transaction.LastExecutionDate;
        
        if (recurrentTransactionLastExecutionDate == null) return 0;
        
        int daysBetween = -(rollbackDate.ToDateTime(new TimeOnly()) - recurrentTransactionLastExecutionDate.Value.ToDateTime(new TimeOnly())).Days;

        var frequencyId = transaction.IntervalTypeId;
        
        switch (frequencyId)
        {
            case { } id when id == _localStorage.IntervalTypes["Daily"]:
                return daysBetween;
        
            case { } id when id == _localStorage.IntervalTypes["Weekly"]:
                return (int)Math.Floor(daysBetween / 7.0);
        
            case { } id when id == _localStorage.IntervalTypes["Monthly"]:
                return (int)Math.Floor(daysBetween / 30.0);
        
            default:
                throw new NotFoundException("Interval Type not found");
        }

    }
}