using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {

            // ✅ Seed SuperAdmin user if not exists
            if (!await context.Users.AnyAsync(u => u.Role == "SuperAdmin"))
            {
                var superAdmin = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "SuperAdmin",
                    FullName = "System SuperAdmin",
                    Email = "superadmin@taskflow.com",
                    PhoneNumber = "00000000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
                    Role = "SuperAdmin",
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(superAdmin);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ SuperAdmin user seeded successfully");
            }
            else
            {
                Console.WriteLine("ℹ️ SuperAdmin already exists — skipping seed");
            }
        }
    }
}
