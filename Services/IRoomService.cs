using RoomService.Models;

namespace RoomService.Services;

public interface IRoomService
{
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int id);
    Task<Room> AddRoomAsync(Room room);
    Task<Room?> UpdateRoomAsync(int id, Room updatedRoom);
    Task<bool> DeleteRoomAsync(int id);
}