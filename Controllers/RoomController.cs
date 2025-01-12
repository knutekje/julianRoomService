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
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
        {
            return NotFound(new { Message = $"Room with ID {id} not found." });
        }

        return Ok(room);
    }

    [HttpGet("room-number/{roomNumber}")]
    public async Task<IActionResult> GetRoomByRoomNumber(string roomNumber)
    {
        var room = await _roomService.GetRoomByRoomNumberAsync(roomNumber);
        if (room == null)
        {
            return NotFound(new { Message = $"Room with number {roomNumber} not found." });
        }

        return Ok(room);
    }

    [HttpGet("dirty")]
    public async Task<IActionResult> GetDirtyRooms()
    {
        var dirtyRooms = await _roomService.GetDirtyRoomsAsync();
        return Ok(dirtyRooms);
    }

    [HttpPost]
    public async Task<IActionResult> AddRoom([FromBody] Room room)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdRoom = await _roomService.AddRoomAsync(room);
        return CreatedAtAction(nameof(GetRoomById), new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var room = await _roomService.UpdateRoomAsync(id, updatedRoom);
        if (room == null)
        {
            return NotFound(new { Message = $"Room with ID {id} not found." });
        }

        return Ok(room);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var success = await _roomService.DeleteRoomAsync(id);
        if (!success)
        {
            return NotFound(new { Message = $"Room with ID {id} not found." });
        }

        return NoContent();
    }
}



