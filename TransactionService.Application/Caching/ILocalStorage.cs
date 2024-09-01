namespace TransactionService.Application.Caching;

public interface ILocalStorage
{
    //properties that i want to cache
    public Dictionary<string,int> Roles { get;  }
    public Dictionary<string, int> IntervalTypes { get; }
    
    public Dictionary<string,int> TransactionTypes { get; }

    public Task LoadEntitiesAsync(CancellationToken cancellationToken);
}