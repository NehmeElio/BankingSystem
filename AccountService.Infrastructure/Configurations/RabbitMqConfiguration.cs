
using AccountService.Infrastructure.Interfaces;
using AccountService.Infrastructure.Services;
using BankingSystem.SharedLibrary.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure.Configurations;

public static class RabbitMqConfiguration
{
    public static void AddRabbitMqService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRabbitMqSenderService<CreateBranchDto>, RabbitMqSenderService<CreateBranchDto>>();
    }
}