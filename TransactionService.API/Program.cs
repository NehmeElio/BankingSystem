
using BankingSystem.SharedLibrary.Configurations;
using BankingSystem.SharedLibrary.DTO;
using Elio.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TransactionService.Application.ActionFilter;
using TransactionService.Application.Configurations;
using TransactionService.Application.Handler;
using TransactionService.Application.Mapper;
using TransactionService.Infrastructure.Configurations;
using TransactionService.Infrastructure.Services;
using TransactionService.Persistence.Configurations;
using UMS_Lab5.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2); // gRPC endpoint
    options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http1AndHttp2); // REST API and Swagger
});

var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");
if (baseUrl != null) builder.WebHost.UseUrls(baseUrl);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbitMqService<CreateBranchDto>(builder.Configuration);

builder.Services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(typeof(WithDrawHandler).Assembly));

builder.AddLoggingServices();

builder.Services.AddDatabaseServices();

builder.Services.AddGlobalExceptionServices();

builder.Services.AddPersistenceServices();

builder.Services.AddAutoMapper(typeof(CustomerMapper));

builder.Services.AddSwaggerServices();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddCaching();

builder.Services.AddScoped<TenantActionFilter>();

builder.Services.AddGrpcServices();

builder.Services.AddRecurrentTransactionConfiguration(builder.Configuration);

builder.Services.AddODataServices();

builder.Services.AddHealthChecksServices(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseGlobalException();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerServices();
}

app.UseHttpsRedirection();

//app.UseTenantMiddleware();

app.UseAuthenticationServices();

app.MapGrpcService<AccountCreationService>();

app.MapControllers();


app.Run();