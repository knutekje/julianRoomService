namespace RoomService.Services;

public interface IMessagePublisher
{
    Task PublishMessageAsync(string message, string queueName);
}