using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using UserManagement.Data;
using UserManagement.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Set up SQL Server Store
var authConnectionString = builder.Configuration["ConnectionStrings:AuthDB"] ?? "";
Console.WriteLine($"Auth Connection String: {authConnectionString}");
builder.Services.AddDbContext<AuthDBContext>(options =>
    options.UseSqlServer(authConnectionString));

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.GetSection("AzureAd");
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AuthDBContext>();
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

                // 1. Extract claims from the Entra token
                var oid = context.Principal?.FindFirstValue("oid");
                var email = context.Principal?.FindFirstValue("preferred_username") ?? context.Principal?.FindFirstValue(ClaimTypes.Email);
                var name = context.Principal?.FindFirstValue("name");

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
                        if (result.Succeeded)
                        {
                            // Optionally assign a default role
                            await userManager.AddToRoleAsync(user, "User");
                        }
                    }

                    // 4. Load roles into the current request identity
                    var roles = await userManager.GetRolesAsync(user);
                    var appIdentity = new ClaimsIdentity();
                    foreach (var role in roles)
                    {
                        appIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                    context.Principal?.AddIdentity(appIdentity);
                }
            }
        };
    }, options => { builder.Configuration.Bind("AzureAd", options); });

// Configure Authorization to use sql server
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AuthDBContext>();
        try
        {
            // Applies any pending migrations
            context.Database.Migrate();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            // Define your roles here
            string[] roleNames = { "Admin", "User", "Manager" };
            foreach (var roleName in roleNames)
            {
                // Check if role exists in the AspNetRoles table
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
