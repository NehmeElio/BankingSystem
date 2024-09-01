using AccountService.Infrastructure.Interfaces;
using AccountService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure.Configurations;

public static class GrpcConfiguration
{
    public static void AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        
        var grpcServerUrl = configuration["GrpcSettings:ServerUrl"];
        
        services.AddSingleton<IGrpcService>(provider => new GrpcService(grpcServerUrl));
    }

}