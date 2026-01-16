using Microsoft.AspNetCore.Identity;
namespace UserManagement.Models
{
  public class Location
  {
    public int Locationid { get; set; }
    public string Name { get; set; } = "";
    public ICollection<UserLocation> UserLocations { get; set; } = new HashSet<UserLocation>();
  }

}