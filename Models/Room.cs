namespace RoomService.Models;



public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty; // e.g., Single, Double, Suite
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = "Available"; // Available, Occupied, Maintenance
    public decimal Price { get; set; }
}
