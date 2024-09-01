using BankingSystem.SharedLibrary.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Caching;
using TransactionService.Application.Commands;
using TransactionService.Application.DTO;
using TransactionService.Application.Validators;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class AddRecurrentTransactionHandler:IRequestHandler<AddRecurrentTransactionCommand>
{
    private readonly ILocalStorage _localStorage;
    private readonly BankContext _dbContext;
    private readonly ILogger<AddRecurrentTransactionHandler> _logger;
    private readonly IValidator<RecurrentTransactionDto> _validator;

    public AddRecurrentTransactionHandler(ILocalStorage localStorage, BankContext dbContext, ILogger<AddRecurrentTransactionHandler> logger, IValidator<RecurrentTransactionDto> validator)
    {
        _localStorage = localStorage;
        _dbContext = dbContext;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task Handle(AddRecurrentTransactionCommand request, CancellationToken cancellationToken)
    {
        var nextExecutionDate = DateOnly.FromDateTime(DateTime.Now);
        switch (request.IntervalType)
        {
            case "Daily":
                nextExecutionDate = nextExecutionDate.AddDays(1);
                break;
            case "Weekly":
                nextExecutionDate = nextExecutionDate.AddDays(7);
                break;
            case "Monthly":
                nextExecutionDate = nextExecutionDate.AddMonths(1);
                break;
            default:
                _logger.LogWarning($"Unknown interval type: {request.IntervalType}");
                break;
        }

        var account=await _dbContext.Accounts.FirstAsync(a => a.Username == request.RecurrentTransactionDto.Username, cancellationToken: cancellationToken);

        if (account == null) throw new NotFoundException("User not found in the current branch");
        
        var accountId=account.AccountId;
        
        RecurrentTransaction recurrentTransaction = new RecurrentTransaction
        {
            AccountId = accountId,
            //Balance = request.RecurrentTransactionDto.Balance,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            IntervalTypeId = _localStorage.IntervalTypes[request.IntervalType],
            IntervalValue = request.RecurrentTransactionDto.IntervalValue,
            DisabledDate = null,
            NextExecutionDate = nextExecutionDate,
            LastExecutionDate = null,
            Version = 0
        };
        
        await _dbContext.RecurrentTransactions.AddAsync(recurrentTransaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Recurrent transaction added successfully");

    }
}