using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Application;
using MediatR;
using TaskFlow.Application.Behaviors;

namespace TaskFlow.Tests.Injection_Tools
{
    public class SharedTestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public ApplicationDbContext DbContext { get; private set; }
        public Guid DefaultUserId { get; private set; }

        public SharedTestFixture()
        {
            var services = new ServiceCollection();

            // In-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            // Logging (required by AutoMapper and other services)
            services.AddLogging();

            // Register repositories and UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register AutoMapper (all profiles in Application assembly)
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(AssemblyMarker).Assembly);
            });

            // Register MediatR handlers from Application assembly
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(AssemblyMarker).Assembly);
            });

            // Build service provider
            ServiceProvider = services.BuildServiceProvider();

            // Get DbContext
            DbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();

            SeedDefaultUser();
        }

        public void Dispose()
        {
            DbContext?.Database.EnsureDeleted();
            DbContext?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void SeedDefaultUser()
        {
            DefaultUserId = Guid.NewGuid();

            DbContext.Users.Add(new User
            {
                Id = DefaultUserId,
                Username = "integration_user",
                FullName = "Integration Test User",
                Email = "integration@test.com",
                PhoneNumber = "0000000000",
                PasswordHash = "hashedpassword",
                Role = Domain.Enums.UserRole.User
            });

            DbContext.SaveChanges();
        }
    }
}
