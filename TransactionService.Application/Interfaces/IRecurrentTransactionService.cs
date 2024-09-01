namespace TransactionService.Application.Interfaces;

public interface IRecurrentTransactionService
{
    Task ProcessRecurrentTransactionsAsync(CancellationToken cancellationToken);
}