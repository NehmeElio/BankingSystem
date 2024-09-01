using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Models;


namespace TransactionService.Application.Commands;

public record GetTransactionsCommand(string Username): IRequest<List<Transaction>>;