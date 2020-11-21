using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext dataContext)
        {
           if (await dataContext.Users.AnyAsync()) return;

           string userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
           var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

           foreach (AppUser user in users)
           {
               using var hmac = new HMACSHA512();
               user.UserName = user.UserName.ToString();
               user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
               user.PasswordSalt = hmac.Key;

               dataContext.Users.Add(user);
           }

           await dataContext.SaveChangesAsync();
        }
    }
}