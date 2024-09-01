using TransactionService.Persistence.Context;

namespace TransactionService.Application.Interfaces;

public interface IBankContextFactory
{
   BankContext CreateDbContext(string schema);
}