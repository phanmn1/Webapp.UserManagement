using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserManagement.Data
{
  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
  {
    public AppDBContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();

      optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AppDb_DesignTime;Trusted_Connection=True;");

      return new AppDBContext(optionsBuilder.Options);
    }
  }
}