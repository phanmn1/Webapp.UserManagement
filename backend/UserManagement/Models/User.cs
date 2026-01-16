using Microsoft.AspNetCore.Identity;
namespace UserManagement.Models
{
    public class User : IdentityUser
    {
        public Guid ExternalId { get; set; }
        public ICollection<UserLocation> UserLocations { get; set; } = new HashSet<UserLocation>();
    }
}
