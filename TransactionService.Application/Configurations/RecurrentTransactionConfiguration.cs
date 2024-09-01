using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Application.BackgroundService;
using TransactionService.Application.DTO;
using TransactionService.Application.Factory;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Services;
using TransactionService.Application.Validators;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Application.Configurations;

public static class RecurrentTransactionConfiguration
{
    public static void AddRecurrentTransactionConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<RecurrentTransactionBackgroundService>();
       services.AddScoped<IBankContextFactory, BankContextFactory>();
        services.AddScoped<IRecurrentTransactionService, RecurrentTransactionService>();
       services.AddValidatorsFromAssemblyContaining<AddRecurrentTransactionValidator>();


    }
}