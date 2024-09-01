using AccountService.Application.ActionFilter;
using AccountService.Application.Configurations;
using AccountService.Application.FluentValidator;
using AccountService.Application.Handler;
using AccountService.Application.Middleware;
using AccountService.Infrastructure.Configurations;
using AccountService.Persistence.Configurations;
using BankingSystem.SharedLibrary.Configurations;
using FluentValidation;
using Elio.Logging;
using UMS_Lab5.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");

if (baseUrl != null) builder.WebHost.UseUrls(baseUrl);

builder.Services.AddScoped<TenantActionFilter>();
builder.Services.AddControllers(
    );

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDatabaseServices();

builder.Services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

builder.Services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(typeof(CreateBranchHandler).Assembly));

builder.Services.AddGlobalExceptionServices();

builder.AddLoggingServices();

builder.Services.AddRabbitMqService(builder.Configuration);

builder.Services.AddPersistenceServices();

builder.Services.AddSwaggerServices();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddGrpcServices(builder.Configuration);

builder.Services.AddCaching();

builder.Services.AddHealthChecksServices(builder.Configuration);

var app = builder.Build();

app.UseGlobalException();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerServices();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//app.UseMiddleware<TenantMiddleware>();

app.UseAuthenticationServices();

app.MapControllers();

app.Run();