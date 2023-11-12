using Api.Data;
using Api.Entities;
using Api.Extensions;
using Api.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);







var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var Services = scope.ServiceProvider;
var context= Services.GetRequiredService<DataContext>();
var userManager=Services.GetRequiredService<UserManager<AppUser>>();
var roleManager=Services.GetRequiredService<RoleManager<AppRole>>();

var logger=Services.GetRequiredService<ILogger<Program>>();
try
{
   await context.Database.MigrateAsync();
   await Seed.SeedUser(userManager,roleManager);
}
catch (Exception ex)
{

    logger.LogError(ex, "Error Occured while Migrating Process");
}

app.Run();
