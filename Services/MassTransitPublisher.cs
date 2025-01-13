using MassTransit;

namespace RoomService.Services;

public class MassTransitPublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T message) where T : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await _publishEndpoint.Publish(message);
    }
}

