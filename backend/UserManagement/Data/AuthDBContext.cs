using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
namespace UserManagement.Data
{
    public class AuthDBContext: IdentityDbContext<User>
    {
        public AuthDBContext(DbContextOptions options) : base(options)
        {
        }

    }
}
