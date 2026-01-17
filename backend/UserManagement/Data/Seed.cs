using Bogus;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class Seed
{
  public static async Task SeedLocation(AppDBContext context, bool _, CancellationToken ct)
  {
    bool exists = await context.Location.AnyAsync();
    if (exists) return;

    List<Location> locations = GenerateFakeLocations();

    foreach (Location location in locations)
    {
      await context.AddAsync(location);
    }
    await context.SaveChangesAsync();
  }
  private static List<Location> GenerateFakeLocations()
  {
    return new Faker<Location>()
      .RuleFor(x => x.Name, f => f.Company.CompanyName())
      .Generate(new Random().Next(10, 20));
  }
}