using System.Security.Claims;
using MediatR;
using TransactionService.Application.DTO;

namespace TransactionService.Application.Commands;

public record DepositCommand(decimal Amount,ClaimsPrincipal User) : IRequest;