using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
namespace UserManagement.Data
{
    public class AuthDBContext : IdentityDbContext<User>
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Set Schema for Auth Tables
            builder.HasDefaultSchema("security");

            // Tell the Security context: "I don't care about these tables"
            builder.Ignore<Location>();
            builder.Ignore<UserLocation>();
        }

    }
}
