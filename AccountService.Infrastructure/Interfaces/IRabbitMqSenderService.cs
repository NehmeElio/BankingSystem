namespace AccountService.Infrastructure.Interfaces;

public interface IRabbitMqSenderService<T>
{
    public void PublishMessage(T message);
}