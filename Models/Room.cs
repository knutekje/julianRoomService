namespace RoomService.Models;



public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty; 
    public int Capacity { get; set; }
    public string Status { get; set; } = "Available"; 
    public decimal Price { get; set; }
}
