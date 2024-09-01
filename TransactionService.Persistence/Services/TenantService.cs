using TransactionService.Domain.Models;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Persistence.Services;

public class TenantService:ITenantService
{
    private  Tenant? _tenant;
    public Tenant? GetTenant()
    {
        return _tenant;
    }

    public void SetTenant(string branch, string user)
    {
        _tenant ??= new Tenant() { Branch = branch, User = user };
        _tenant.Branch = branch;
        _tenant.User = user;
    }

    public void SetUser(string user)
    {
        _tenant ??= new Tenant() { User = user };
        _tenant.User = user;
    }
    
    public void SetBranch(string branch)
    {
        _tenant ??= new Tenant() { Branch = branch};
        _tenant.Branch = branch;
    }
}