
using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Caching;
using TransactionService.Application.Commands;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class DepositHandler:IRequestHandler<DepositCommand>
{
    private readonly BankContext _dbContext;
    private readonly ILogger<DepositHandler> _logger;
    private readonly ILocalStorage _localStorage;

    public DepositHandler(BankContext dbContext, ILogger<DepositHandler> logger, ILocalStorage localStorage)
    {
        _dbContext = dbContext;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task Handle(DepositCommand request, CancellationToken cancellationToken)
    {
       var username=request.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value; 
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken: cancellationToken);
        
        if(account==null) throw new UnauthorizedAccessException($"Access denied");
        if(request.Amount<0) throw new InvalidAmountException("Invalid amount it cannot be negative");
        
        account.Balance += request.Amount;
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"User {username} deposited {request.Amount} and now has {account.Balance}");


        
        Transaction transaction = new Transaction
        {
            AccountId = await _dbContext.Accounts
                .Where(a=>a.Username==username)
                .Select(a=>a.AccountId)
                .FirstOrDefaultAsync(cancellationToken),
            TransactionTypeId = _localStorage.TransactionTypes["Deposit"],
            Amount = request.Amount,
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            Timestamp = DateTime.Now,
        };
        
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Transaction {transaction.TransactionId} created for user {username}");



    }
}