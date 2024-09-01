using BankingSystem.SharedLibrary.DTO;
using MediatR;

namespace AccountService.Application.Commands;

public record CreateEmployeeCommand(AddUserDto AddUserDto) : IRequest;