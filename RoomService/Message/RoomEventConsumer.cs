namespace RoomService.Message;

using MassTransit;
using RoomService.Message;


public class RoomEventConsumer : IConsumer<RoomEvent>
{
    public async Task Consume(ConsumeContext<RoomEvent> context)
    {
        var message = context.Message;
        Console.WriteLine($"Received Room Event: RoomId={message.RoomId}, Action={message.Action}");

        // Add your business logic here
        if (message.Action == "MarkDirty")
        {
            Console.WriteLine($"Room {message.RoomId} marked as dirty.");
        }
        else if (message.Action == "MarkClean")
        {
            Console.WriteLine($"Room {message.RoomId} marked as clean.");
        }

        await Task.CompletedTask; // Placeholder for async operations
    }
}
