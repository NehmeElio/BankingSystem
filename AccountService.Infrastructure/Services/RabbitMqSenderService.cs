using System.Text;
using System.Text.Json;
using AccountService.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace AccountService.Infrastructure.Services;

public class RabbitMqSenderService<T>:IRabbitMqSenderService<T>
{
    private readonly string? _hostname;
    private readonly string? _queueName;
    private readonly IConnection _connection;

    public RabbitMqSenderService(IConfiguration configuration)
    {
        _hostname = configuration["RabbitMQ:HostName"];
        _queueName = configuration["RabbitMQ:QueueName"];
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName="guest",
            Password = "guest",
            VirtualHost = "/",
        };
        _connection = factory.CreateConnection();
    }

    public void PublishMessage(T objectToSend)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        var jsonString = JsonSerializer.Serialize(objectToSend);
        
        var body = Encoding.UTF8.GetBytes(jsonString);
        channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
    }
}