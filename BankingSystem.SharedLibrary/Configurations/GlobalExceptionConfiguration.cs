using BankingSystem.SharedLibrary.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.SharedLibrary.Configurations;

public static class GlobalExceptionConfiguration
{
    public static void AddGlobalExceptionServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
    }

    public static void UseGlobalException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
     {
         errorApp.Run(async context =>
         {
             var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
             var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
             var exception = exceptionFeature?.Error;

             if (exception != null)
             {
                 await exceptionHandler.TryHandleAsync(context, exception, context.RequestAborted);
             }
         });
     }); 
    }
    
}