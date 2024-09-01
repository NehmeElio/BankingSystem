
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Persistence.Interfaces;
using TransactionService.Persistence.Services;

namespace TransactionService.Persistence.Configurations;

public static class PersistenceServicesConfiguration
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<IBranchSchemaService, BranchSchemaService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IModifyConnectionService, ModifyConnectionService>();
    }
}