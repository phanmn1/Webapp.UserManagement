using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserManagement.Data
{
  public class AuthDBContextFactory : IDesignTimeDbContextFactory<AuthDBContext>
  {
    public AuthDBContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<AuthDBContext>();

      // This string only needs to be valid syntactically for the tool to work.
      // It doesn't need to be your real Docker/Production string.
      optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AuthDb_DesignTime;Trusted_Connection=True;");

      return new AuthDBContext(optionsBuilder.Options);
    }
  }
}