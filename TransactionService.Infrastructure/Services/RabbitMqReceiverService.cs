using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using BankingSystem.SharedLibrary.DTO;
using BankingSystem.SharedLibrary.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Infrastructure.Services;

public class RabbitMqReceiverService<T> : BackgroundService
{
    private readonly ILogger<RabbitMqReceiverService<T>> _logger;
    private readonly IConnection _connection;
    private readonly string? _queueName;
    private readonly IServiceProvider _serviceProvider;

    private readonly IBranchSchemaService _branchSchemaService;
    //private readonly BankContext _context;

    public RabbitMqReceiverService(ILogger<RabbitMqReceiverService<T>> logger, IConfiguration configuration,
        IServiceProvider serviceProvider, IBranchSchemaService branchSchemaService)
    {
        _logger = logger;
        _queueName = configuration["RabbitMQ:QueueName"];
        _serviceProvider = serviceProvider;
        _branchSchemaService = branchSchemaService;
        //_context = context;

        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:HostName"],
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        _connection = factory.CreateConnection();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {Message}", message);

                // Process the message
                await ProcessBranchCreationMessage(message);
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            // Keep the task running
            stoppingToken.WaitHandle.WaitOne();
        }, stoppingToken);
    }

    private async Task ProcessBranchCreationMessage(string message)
    {
        
        T? newEntity = JsonSerializer.Deserialize<T>(message);
        _logger.LogInformation("Successfully deserialized message into type {ObjectType}", typeof(T).Name);
        
        if (newEntity is CreateBranchDto newBranch)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();
            
            var newBranchName = newBranch.BranchName;
        
            var branchExists=await dbContext.Branches.AnyAsync(x=>x.BranchName==newBranchName);
        
            if(branchExists) throw new DuplicateException("The branch name "+newBranchName+" already exists");

            await dbContext.Branches.AddAsync(new Branch()
            {
                BranchName = newBranch.BranchName,
                Address = newBranch.Address
            });
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully added branch: {BranchName} to the table Branches", newBranch.BranchName);

            if (newBranch.BranchName != null)
            {
                _branchSchemaService.CreateBranchSchema(newBranch.BranchName, "beirutbranch");

                _logger.LogInformation("Successfully created branch schema for branch: {BranchName}",
                    newBranch.BranchName);
            }
        }
        else
        {
            _logger.LogWarning("The deserialized object is not of type Branch. Received type: {ObjectType}", typeof(T).Name);
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _connection.Close();
        return base.StopAsync(stoppingToken);
    }
}