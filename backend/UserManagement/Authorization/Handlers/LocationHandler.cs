using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using UserManagement.Data;

namespace UserManagement.Authorization;

public class LocationHandler : AuthorizationHandler<UserInSameLocation>
{
  private readonly AppDBContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public LocationHandler(AppDBContext dbContext, IHttpContextAccessor httpContextAccessor)
  {
    _context = dbContext;
    _httpContextAccessor = httpContextAccessor;
  }

  protected override async Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      UserInSameLocation requirement)
  {
    // 1. Get the User ID from the Claims (using the 'oid' or NameIdentifier)
    var externalUserId = context.User.GetObjectId();

    Console.WriteLine($"UserID: {externalUserId}");
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext == null || string.IsNullOrEmpty(externalUserId))
      return;

    var externalUserIdGuid = Guid.Parse(externalUserId);

    var user = await _context.User.SingleOrDefaultAsync(u => u.ExternalId == externalUserIdGuid);
    if (user == null) return;

    // Pull {locationId} from Route Data
    if (httpContext.Request.RouteValues.TryGetValue("locationId", out var objValue) &&
      int.TryParse(objValue?.ToString(), out int locationId)
    )
    {
      // 2. Check if the mapping exists in the database
      var hasAccess = await _context.UserLocations
        .AnyAsync(ul => ul.UserId == user.Id && ul.LocationId == locationId);

      if (hasAccess)
      {
        context.Succeed(requirement);
      }
    }

    return;

  }
}