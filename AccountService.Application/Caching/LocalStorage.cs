using AccountService.Persistence.Context;
using BankingSystem.SharedLibrary.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UMS_Lab5.Caching;

namespace AccountService.Application.Caching;

public class LocalStorage:ILocalStorage
{
    private readonly IServiceProvider _service;
    private readonly ICachingManager _cache;
    public LocalStorage(IServiceProvider service, ICachingManager cache)
    {
        _service = service;
        _cache = cache;
    }
    public Dictionary<string,int> Roles=>
        _cache.Get<Dictionary<string,int>>(ApplicationConstants.CachingKeys.Role);

    public async Task LoadEntitiesAsync(CancellationToken cancellationToken)
    {
        using (var scope = _service.CreateScope())
        {
            var context=scope.ServiceProvider.GetRequiredService<BankContext>();
            
            _cache.Add(ApplicationConstants.CachingKeys.Role,
                await context.Roles.ToDictionaryAsync(x => x.RoleName, x => x.RoleId, cancellationToken));
            
        }
    }
}