using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Api.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;

namespace Api.Data
{
    public class Seed
    {
        
        public static async Task SeedUser(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager
        )
        {
             if(userManager.Users.Any()) return;
            
            var userData = File.ReadAllText("Data/UserSeedData.json");

             var options = new JsonSerializerOptions{PropertyNameCaseInsensitive=true};
           
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles= new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {

             user.UserName=user.UserName.ToLower();        
            await userManager.CreateAsync(user,"Pa$$w0rd");
            await userManager.AddToRoleAsync(user,"Member");

            }

            var admin=new AppUser
            {
              UserName="Admin"
            };

            await userManager.CreateAsync(admin,"Pa$$w0rd");
             await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}