namespace AccountService.Application.Caching;

public interface ILocalStorage
{
    //properties that i want to cache
    public Dictionary<string,int> Roles { get;  }

    public Task LoadEntitiesAsync(CancellationToken cancellationToken);
}