using AutoMapper;
using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commands;
using TransactionService.Application.ViewModel;
using TransactionService.Persistence.Context;

namespace TransactionService.Application.Handler;

public class GetAccountInformationHandler:IRequestHandler<GetAccountInformationCommand,AccountViewModel>
{
    private readonly BankContext _context;
    private readonly ILogger<GetAccountInformationCommand> _logger;
    private readonly IMapper _mapper;

    public GetAccountInformationHandler(BankContext context, ILogger<GetAccountInformationCommand> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AccountViewModel> Handle(GetAccountInformationCommand request, CancellationToken cancellationToken)
    {
        var account=await _context.Accounts.FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken: cancellationToken);
        if (account == null)
        {
            _logger.LogError("Account not found");
            throw new NotFoundException($"Account {request.Username} not found");
        }
        
        return _mapper.Map<AccountViewModel>(account);
        
    }
}