using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using UserManagement.Models;
using UserManagement.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Extensions;

public static class AuthenticationExtensions
{
  public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddMicrosoftIdentityWebApi(options =>
    {
      configuration.GetSection("AzureAd");

      // Don't remap JWT Claim from v2.0 to v1.0 SAML
      options.MapInboundClaims = false;

      options.Events = new JwtBearerEvents
      {
        OnTokenValidated = async context =>
        {
          var dbContext = context.HttpContext.RequestServices.GetRequiredService<AuthDBContext>();
          var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

          // 1. Extract claims from the Entra token
          var oid = context.Principal?.FindFirstValue("oid");
          var email = context.Principal?.FindFirstValue("email");
          var name = context.Principal?.FindFirstValue("name");

          Console.WriteLine($"Email: {email}");
          Console.WriteLine($"Name: {name}");
          Console.WriteLine($"OID: {oid}");

          if (!string.IsNullOrEmpty(oid))
          {
            // 2. Check if user exists by ExternalId (oid)
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.ExternalId == Guid.Parse(oid));
            if (user == null)
            {
              // 3. User doesn't exist - Create them!
              user = new User
              {
                UserName = email,
                Email = email,
                ExternalId = Guid.Parse(oid),
                EmailConfirmed = true // Trusted because it's from Entra ID
              };

              var result = await userManager.CreateAsync(user);
              Console.WriteLine($"Result: {result.Succeeded}");
              if (result.Succeeded)
              {
                // Optionally assign a default role
                await userManager.AddToRoleAsync(user, "User");
              }


            }

            // 4. Load roles into the current request identity
            var roles = await userManager.GetRolesAsync(user);
            var appIdentity = new ClaimsIdentity();

            // Fix token with claims
            foreach (var role in roles)
            {
              appIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            context.Principal?.AddIdentity(appIdentity);
          }
        }
      };
    }, options => { configuration.Bind("AzureAd", options); });

    return services;
  }

}