using BankingSystem.SharedLibrary.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BankingSystem.SharedLibrary.Configurations;

public static class AuthenticationConfiguration
{
    public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        
        var authenticationSettings = new AuthenticationSettings();
        configuration.GetSection(nameof(AuthenticationSettings)).Bind(authenticationSettings);
        services.AddSingleton(authenticationSettings); 
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = authenticationSettings.Authority;
                options.Audience = authenticationSettings.Audience;
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
        
                        return Task.CompletedTask;
                    },
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal == null) return Task.CompletedTask;
                        var claims = context.Principal.Claims;
                        foreach (var claim in claims)
                        {
                            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void UseAuthenticationService(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

}