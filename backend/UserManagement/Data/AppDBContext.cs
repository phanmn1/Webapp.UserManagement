using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
  public class AppDBContext : DbContext
  {
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    public DbSet<Location> Location { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      // Set Schema for Auth Tables
      builder.HasDefaultSchema("dbo");

      builder.Entity<UserLocation>(entity =>
      {
        entity.ToTable("UserLocations", "dbo");

        entity.HasKey(ul => ul.Id);

        // Define the relationship to user table in the OTHER schema
        // Link User to UserLocation
        entity.HasOne<User>()
              .WithMany(u => u.UserLocations) // This is the key change
              .HasForeignKey(ul => ul.UserId);

        // Link Location to UserLocation
        entity.HasOne<Location>()
              .WithMany(l => l.UserLocations) // This matches the change in Location class
              .HasForeignKey(ul => ul.LocationId);
      });

      // Remind this context that ApplicationUser lives in the security schema
      builder.Entity<User>().ToTable("AspNetUsers", "security")
        .Metadata
        .SetIsTableExcludedFromMigrations(true);

    }

  }
}