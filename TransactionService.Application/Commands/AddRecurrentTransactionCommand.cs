using MediatR;
using TransactionService.Application.DTO;

namespace TransactionService.Application.Commands;

public record AddRecurrentTransactionCommand(RecurrentTransactionDto RecurrentTransactionDto,string IntervalType) : IRequest;