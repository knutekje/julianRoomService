using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;

namespace RoomService.Services;

public class RoomService : IRoomService
{
    private readonly RoomDbContext _context;

    public RoomService(RoomDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int id)
    {
        return await _context.Rooms.FindAsync(id);
    }

    public async Task<Room> AddRoomAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<Room?> UpdateRoomAsync(int id, Room updatedRoom)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return null;
        }

        room.RoomNumber = updatedRoom.RoomNumber;
        room.RoomType = updatedRoom.RoomType;
        room.Capacity = updatedRoom.Capacity;
        room.IsAvailable = updatedRoom.IsAvailable;
        room.Status = updatedRoom.Status;
        room.Price = updatedRoom.Price;

        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return false;
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
}