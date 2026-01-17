using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Auth;

public class LocationPermissionHandler : AuthorizationHandler<MustBelongToLocationAttribute>
{
  private readonly AppDBContext _context;

  public LocationPermissionHandler(AppDBContext context)
  {
    _context = context;
  }

  protected override async Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      MustBelongToLocationAttribute requirement)
  {
    // 1. Get the User ID from the Claims (using the 'oid' or NameIdentifier)
    var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return;
    }

    // 2. Check if the mapping exists in the database
    var hasAccess = await _context.UserLocations
      .AnyAsync(ul => ul.UserId == userId && ul.LocationId == requirement.LocationId);

    if (hasAccess)
    {
      context.Succeed(requirement);
    }
  }
}