using BankingSystem.SharedLibrary.DTO;
using MediatR;

namespace AccountService.Application.Commands;

public record CreateAccountCommand(AddAccountDto AddAccountDto):IRequest;