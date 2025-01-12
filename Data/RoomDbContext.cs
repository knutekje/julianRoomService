using Microsoft.EntityFrameworkCore;
using RoomService.Models;

namespace RoomService.Data;

public class RoomDbContext : DbContext
{
    public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .Property(r => r.Status)
            .HasConversion<string>(); // Store enum as string in the database
    }
}