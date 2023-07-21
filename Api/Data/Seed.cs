using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Api.Entities;

namespace Api.Data
{
    public class Seed
    {
        
        public static async Task SeedUser(DataContext context)
        {
             if(context.Users.Any()) return;
            
            var userData = File.ReadAllText("Data/UserSeedData.json");

             var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
           
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach(var user in users)
            {

             using var hmac=new HMACSHA512();

             user.UserName=user.UserName.ToLower();
             user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
             user.PasswordSalt=hmac.Key;

              context.Users.Add(user);

            }

            await context.SaveChangesAsync();
            
        }
    }
}