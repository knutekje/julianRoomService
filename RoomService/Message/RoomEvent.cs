namespace RoomService.Message;

public class RoomEvent
{
    public int RoomId { get; set; }
    public string Action { get; set; } // e.g., "MarkDirty", "MarkClean", etc.
}