public class UserLocation
{
  public Guid Id { get; set; }
  public string UserId { get; set; } // Points to security.AspNetUsers
  public int LocationId { get; set; } // Points to dbo.Locations

}