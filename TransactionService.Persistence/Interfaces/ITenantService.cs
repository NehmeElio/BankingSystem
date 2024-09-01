using TransactionService.Domain.Models;

namespace TransactionService.Persistence.Interfaces;

public interface ITenantService
{
    Tenant? GetTenant();
    void SetTenant(string branch,string user);

    void SetUser(string user);
    
    void SetBranch(string branch);
}