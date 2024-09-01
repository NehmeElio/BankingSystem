
using AutoMapper;
using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commands;
using TransactionService.Application.ViewModel;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class GetTransactionsHandler:IRequestHandler<GetTransactionsCommand,List<Transaction>>
{
    private readonly BankContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTransactionsCommand> _logger;

    public GetTransactionsHandler(BankContext context, IMapper mapper, ILogger<GetTransactionsCommand> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<Transaction>> Handle(GetTransactionsCommand request, CancellationToken cancellationToken)
    {
        var account=await _context.Accounts
            .FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken: cancellationToken);
        
        if(account==null) throw new NotFoundException($"Account {request.Username} not found");
        
        var transactions = await _context.Transactions.Where(x => x.AccountId == account.AccountId)
            .ToListAsync(cancellationToken);

        return transactions;
    }
}