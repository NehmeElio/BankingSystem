using MediatR;
using TransactionService.Application.DTO;

namespace TransactionService.Application.Commands;

public record RollbackCommand(RollbackDto RollbackDto):IRequest;