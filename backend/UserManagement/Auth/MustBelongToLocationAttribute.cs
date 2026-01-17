using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Auth;
// This is both the Attribute you put on Controllers AND the Requirement logic
public class MustBelongToLocationAttribute : Attribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
  public int LocationId { get; }

  public MustBelongToLocationAttribute(int locationId)
  {
    LocationId = locationId;
  }

  // This satisfies IAuthorizationRequirementData
  public IEnumerable<IAuthorizationRequirement> GetRequirements()
  {
    yield return this;
  }
}