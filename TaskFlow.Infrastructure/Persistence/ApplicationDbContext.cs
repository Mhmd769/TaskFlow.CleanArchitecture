using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<TaskAssignedUser> TaskAssignedUsers => Set<TaskAssignedUser>();
        public DbSet<Notification> Notifications => Set<Notification>();


        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Conversation> Conversations => Set<Conversation>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project -> Owner
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Remove old single-user assignment relation
            // modelBuilder.Entity<TaskItem>()
            //     .HasOne(t => t.AssignedUser)
            //     .WithMany(u => u.Tasks)
            //     .HasForeignKey(t => t.AssignedUserId)
            //     .OnDelete(DeleteBehavior.SetNull);

            // Configure many-to-many: TaskItem <-> User through TaskAssignedUser
            modelBuilder.Entity<TaskAssignedUser>()
                .HasKey(tu => new { tu.TaskId, tu.UserId });

            modelBuilder.Entity<TaskAssignedUser>()
                .HasOne(tu => tu.Task)
                .WithMany(t => t.AssignedUsers)
                .HasForeignKey(tu => tu.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskAssignedUser>()
                .HasOne(tu => tu.User)
                .WithMany()
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role enum as string
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();


            // Message config
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Content)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.HasIndex(m => new { m.SenderId, m.ReceiverId });
                entity.HasIndex(m => m.CreatedAt);
            });

            // Conversation config
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasIndex(c => new { c.User1Id, c.User2Id })
                      .IsUnique();

                entity.HasIndex(c => c.LastMessageAt);
            });
        }
    }
}
