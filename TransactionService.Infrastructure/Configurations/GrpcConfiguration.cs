using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TransactionService.Infrastructure.Configurations;

public static class GrpcConfiguration
{
    public static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
    }


}