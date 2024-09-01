using AccountService.Persistence.Interfaces;
using AccountService.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Persistence.Configurations;

public static class PersistenceServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<IBranchSchemaService, BranchSchemaService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IModifyConnectionService, ModifyConnectionService>();
        
        return services;
    }
}