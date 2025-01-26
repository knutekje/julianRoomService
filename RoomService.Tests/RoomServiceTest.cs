using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;
using RoomService.Services;
using Xunit;
using FluentAssertions;

namespace RoomService.Tests
{
    public class RoomServiceTests
    {
        private readonly RoomDbContext _context;
        private readonly Services.RoomService _roomService;

        public RoomServiceTests()
        {
            var options = new DbContextOptionsBuilder<RoomDbContext>()
                .UseInMemoryDatabase(databaseName: "RoomServiceTestDb")
                .Options;

            _context = new RoomDbContext(options);
            _roomService = new Services.RoomService(_context);
        }
        
        
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        
        [Fact]
        public async Task AddRoomAsync_ShouldAddRoomSuccessfully()
        {
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Single",
                Capacity = 1,
                Status = RoomStatus.Clean,
                Price = 100.0m
            };

            var addedRoom = await _roomService.AddRoomAsync(room);

            var dbRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == addedRoom.Id);
            dbRoom.Should().NotBeNull();
            dbRoom!.RoomNumber.Should().Be("101");
            dbRoom.Status.Should().Be(RoomStatus.Clean);
            Dispose();
        }

        [Fact]
        public async Task GetAllRoomsAsync_ShouldReturnAllRooms()
        {
            _context.Rooms.Add(new Room { RoomNumber = "101", RoomType = "Single", Capacity = 1, Status = RoomStatus.Clean, Price = 100.0m });
            _context.Rooms.Add(new Room { RoomNumber = "102", RoomType = "Double", Capacity = 2, Status = RoomStatus.Dirty, Price = 200.0m });
            await _context.SaveChangesAsync();

            var rooms = await _roomService.GetAllRoomsAsync();

            rooms.Should().HaveCount(2);
            Dispose();
        }

        [Fact]
        public async Task UpdateRoomAsync_ShouldUpdateRoomSuccessfully()
        {
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Single",
                Capacity = 1,
                Status = RoomStatus.Clean,
                Price = 100.0m
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var updatedRoom = new Room
            {
                Id = room.Id,
                RoomNumber = "101A",
                RoomType = "Deluxe",
                Capacity = 2,
                Status = RoomStatus.Dirty,
                Price = 150.0m
            };

            var result = await _roomService.UpdateRoomAsync(room.Id, updatedRoom);

            result.Should().NotBeNull();
            result!.RoomType.Should().Be("Deluxe");
            result.Status.Should().Be(RoomStatus.Dirty);
            result.Price.Should().Be(150.0m);
            Dispose();
        }

        [Fact]
        public async Task DeleteRoomAsync_ShouldDeleteRoomSuccessfully()
        {
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Single",
                Capacity = 1,
                Status = RoomStatus.Clean,
                Price = 100.0m
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var result = await _roomService.DeleteRoomAsync(room.Id);

            result.Should().BeTrue();
            var dbRoom = await _context.Rooms.FindAsync(room.Id);
            dbRoom.Should().BeNull();
            Dispose();
        }

        [Fact]
        public async Task GetRoomByRoomNumberAsync_ShouldReturnCorrectRoom()
        {
            var room = new Room
            {
                RoomNumber = "101",
                RoomType = "Single",
                Capacity = 1,
                Status = RoomStatus.Clean,
                Price = 100.0m
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var result = await _roomService.GetRoomByRoomNumberAsync("101");

            result.Should().NotBeNull();
            result!.RoomType.Should().Be("Single");
            result.Status.Should().Be(RoomStatus.Clean);
            Dispose();
        }

        [Fact]
        public async Task GetDirtyRoomsAsync_ShouldReturnOnlyDirtyRooms()
        {
            _context.Rooms.Add(new Room { RoomNumber = "101", RoomType = "Single", Capacity = 1, Status = RoomStatus.Dirty, Price = 100.0m });
            _context.Rooms.Add(new Room { RoomNumber = "102", RoomType = "Double", Capacity = 2, Status = RoomStatus.Clean, Price = 200.0m });
            await _context.SaveChangesAsync();

            var dirtyRooms = await _roomService.GetDirtyRoomsAsync();

            dirtyRooms.Should().HaveCount(1);
            dirtyRooms.First().RoomNumber.Should().Be("101");
            Dispose();
        }
    }
}
