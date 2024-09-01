using AccountService.Infrastructure.Interfaces;
using AccountService.Infrastructure.Services;
using AccountService.Infrastructure.Validators;
using BankingSystem.SharedLibrary.Configurations;
using BankingSystem.SharedLibrary.Helper;
using BankingSystem.SharedLibrary.Interfaces;
using BankingSystem.SharedLibrary.Models;
using BankingSystem.SharedLibrary.Services;
using BankingSystem.SharedLibrary.Settings;
using FluentValidation;
using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure.Configurations;

public static class AuthenticationConfiguration
{
    public static void AddAuthenticationServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpClient();
        
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAddKeycloakAccountService, AddKeycloakAccountService>();
        
        services.AddAuthenticationService(configuration);
        
        services.AddScoped(sp => 
            new KeycloakClient(
                sp.GetRequiredService<AuthenticationSettings>().ServerUrl,
                sp.GetRequiredService<AuthenticationSettings>().AdminUsername,
                sp.GetRequiredService<AuthenticationSettings>().AdminPassword));

        services.AddTransient<IValidator<User>, KeycloakUserValidator>();
        
        services.AddAuthorization(options =>
        {
            // Policy for Admin role
            options.AddPolicy("RequireAdminRole", policy =>
                policy.Requirements.Add(new RolesRequirement(new List<string> { "admin" })));

            // Policy for Customer role
            options.AddPolicy("RequireCustomerRole", policy =>
                policy.Requirements.Add(new RolesRequirement(new List<string> { "customer" })));

            // Policy for Employee role
            options.AddPolicy("RequireEmployeeRole", policy =>
                policy.Requirements.Add(new RolesRequirement(new List<string> { "employee" })));

            // Combined Policy for Admin, Customer, or Employee roles
            options.AddPolicy("RequireAdminOrCustomerOrEmployee", policy =>
                policy.Requirements.Add(new RolesRequirement(new List<string> { "admin", "customer", "employee" })));
        });

        // Register the custom handler for RolesRequirement
        services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();
    }
    
    public static void UseAuthenticationServices(this IApplicationBuilder app)
    {
        app.UseAuthenticationService();

    }
}