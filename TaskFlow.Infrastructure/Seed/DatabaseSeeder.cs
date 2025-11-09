using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure DB exists
            await context.Database.MigrateAsync();

            // Check if SuperAdmin already exists
            var superAdminEmail = "superadmin@taskflow.com";
            if (await context.Users.AnyAsync(u => u.Email == superAdminEmail))
                return;

            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                Username = "superadmin",
                FullName = "System Super Admin",
                Email = superAdminEmail,
                PhoneNumber = "00000000",
                Role = Domain.Enums.UserRole.SuperAdmin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(superAdmin);
            await context.SaveChangesAsync();
        }
    }
}
