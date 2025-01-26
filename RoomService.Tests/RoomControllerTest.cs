
using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomService.Controllers;
using RoomService.Models;
using RoomService.Services;
using Xunit;
using FluentAssertions;



namespace RoomService.Tests;

public class RoomsControllerTests
{
    private readonly Mock<IRoomService> _mockRoomService;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _mockRoomService = new Mock<IRoomService>();
        _controller = new RoomsController(_mockRoomService.Object);
    }

    [Fact]
    public async Task GetAllRooms_ShouldReturnOkWithRooms()
    {
        var rooms = new List<Room>
        {
            new Room { Id = 1, RoomNumber = "101", RoomType = "Single", Status = RoomStatus.Clean },
            new Room { Id = 2, RoomNumber = "102", RoomType = "Double", Status = RoomStatus.Dirty }
        };

        _mockRoomService.Setup(service => service.GetAllRoomsAsync()).ReturnsAsync(rooms);

        var result = await _controller.GetAllRooms();

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(rooms);
    }

    [Fact]
    public async Task GetRoomById_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        _mockRoomService.Setup(service => service.GetRoomByIdAsync(It.IsAny<int>())).ReturnsAsync((Room?)null);

        var result = await _controller.GetRoomById(1);

        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(new { Message = "Room with ID 1 not found." });
    }

    [Fact]
    public async Task AddRoom_ShouldReturnCreatedRoom()
    {
        var newRoom = new Room { RoomNumber = "101", RoomType = "Single", Status = RoomStatus.Clean };
        var createdRoom = new Room { Id = 1, RoomNumber = "101", RoomType = "Single", Status = RoomStatus.Clean };

        _mockRoomService.Setup(service => service.AddRoomAsync(It.IsAny<Room>())).ReturnsAsync(createdRoom);

        var result = await _controller.AddRoom(newRoom);

        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(createdRoom);
    }

    [Fact]
    public async Task UpdateRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        var updatedRoom = new Room { RoomNumber = "101A", RoomType = "Single", Status = RoomStatus.Clean };

        _mockRoomService.Setup(service => service.UpdateRoomAsync(It.IsAny<int>(), It.IsAny<Room>()))
            .ReturnsAsync((Room?)null);

        var result = await _controller.UpdateRoom(1, updatedRoom);

        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(new { Message = "Room with ID 1 not found." });
    }

    [Fact]
    public async Task DeleteRoom_ShouldReturnNoContent_WhenRoomIsDeleted()
    {
        _mockRoomService.Setup(service => service.DeleteRoomAsync(It.IsAny<int>())).ReturnsAsync(true);

        var result = await _controller.DeleteRoom(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetDirtyRooms_ShouldReturnOkWithDirtyRooms()
    {
        var dirtyRooms = new List<Room>
        {
            new Room { Id = 1, RoomNumber = "101", RoomType = "Single", Status = RoomStatus.Dirty }
        };

        _mockRoomService.Setup(service => service.GetDirtyRoomsAsync()).ReturnsAsync(dirtyRooms);

        var result = await _controller.GetDirtyRooms();

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(dirtyRooms);
    }
}
