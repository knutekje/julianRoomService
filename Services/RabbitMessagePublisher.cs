namespace RoomService.Services;

public class RabbitMessagePublisher : IMessagePublisher
{
    public Task PublishMessageAsync(string message, string queueName)
    {
        throw new NotImplementedException();
    }
}