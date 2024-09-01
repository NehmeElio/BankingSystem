using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Caching;
using TransactionService.Application.Commands;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class WithDrawHandler: IRequestHandler<WithDrawCommand>
{
    private readonly BankContext _dbContext;
    private  readonly ILogger<WithDrawHandler> _logger;
    private readonly ILocalStorage _localStorage;

    public WithDrawHandler(BankContext dbContext, ILogger<WithDrawHandler> logger, ILocalStorage localStorage)
    {
        this._dbContext = dbContext;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task Handle(WithDrawCommand request, CancellationToken cancellationToken)
    {
        var username=request.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var account =await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken: cancellationToken);
        
        if(account==null) throw new UnauthorizedAccessException($"Access denied");
        if(request.Amount<0) throw new InvalidAmountException("Invalid amount it cannot be negative");
        
        account.Balance -= request.Amount;
        if(account.Balance<0) throw new InsufficentBalanceException("Insufficient balance");
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"User {username} withdrew {request.Amount} and is left with {account.Balance}");

        Transaction transaction = new Transaction
        {
            AccountId = await _dbContext.Accounts
                .Where(a=>a.Username==username)
                .Select(a=>a.AccountId)
                .FirstOrDefaultAsync(cancellationToken),
            TransactionTypeId = _localStorage.TransactionTypes["Withdrawal"],
            Amount = request.Amount,
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            Timestamp = DateTime.Now,
        };
        
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Transaction {transaction.TransactionId} created for user {username}");
    }
}