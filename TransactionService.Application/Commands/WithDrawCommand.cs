using System.Security.Claims;
using MediatR;
using TransactionService.Application.DTO;

namespace TransactionService.Application.Commands;

public record WithDrawCommand(decimal Amount,ClaimsPrincipal User) : IRequest;