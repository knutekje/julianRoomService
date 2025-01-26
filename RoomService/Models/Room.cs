namespace RoomService.Models;


public enum RoomStatus
{
    Clean,
    Dirty,
    OutOfService
}

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public RoomStatus Status { get; set; } // Clean, Dirty, OutOfService
    public decimal Price { get; set; }
    public Boolean ExtraBed { get; set; }
    public DateTime? LastMaintenance { get; set; } // Optional: Track maintenance history
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

