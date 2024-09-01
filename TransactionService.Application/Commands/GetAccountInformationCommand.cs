using MediatR;
using TransactionService.Application.ViewModel;

namespace TransactionService.Application.Commands;

public record GetAccountInformationCommand(string Username):IRequest<AccountViewModel>;