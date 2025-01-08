using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomService.Models;
using RoomService.Services;

namespace RoomService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }
        return Ok(room);
    }
    
    /*[Authorize(Roles = "Admin")]
    [HttpGet("debug-token")]
    public IActionResult DebugToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }*/
    
    [HttpPost("add-room")]
    public async Task<ActionResult<Room>> PostRoom(Room room)
    {
        var createdRoom = await _roomService.AddRoomAsync(room);
        return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoom(int id, Room updatedRoom)
    {
        var room = await _roomService.UpdateRoomAsync(id, updatedRoom);
        if (room == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var success = await _roomService.DeleteRoomAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
    [HttpGet("available")]
    public async Task<ActionResult<Room>> GetRoomByRoomNumber(string roomNumber)
    {
        var room = await _roomService.GetRoomByRoomNumberAsync(roomNumber);
        if (room == null)
        {
            return NotFound();
        }
        return Ok(room);
    }
    
  

        [HttpGet("dirty-rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetDirtyRooms()
        {
            var dirtyRooms = await _roomService.GetDirtyRoomsAsync();
            if (!dirtyRooms.Any())
            {
                return NotFound("No dirty rooms found.");
            }

            return Ok(dirtyRooms);
        }
    }

    


