using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using UserManagement.Data;
using UserManagement.Models;
using System.Security.Claims;
using UserManagement.Extensions;

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

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(authConnectionString));

// Add Authentication
builder.Services.AddCustomAuth(builder.Configuration);


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
        var authContext = services.GetRequiredService<AuthDBContext>();
        var appContext = services.GetRequiredService<AppDBContext>();

        try
        {
            // Applies any pending migrations
            authContext.Database.Migrate();
            appContext.Database.Migrate();
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

    using (IServiceScope scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var appContext = services.GetRequiredService<AppDBContext>();

        await Seed.SeedLocation(appContext, false, CancellationToken.None);
    }

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
