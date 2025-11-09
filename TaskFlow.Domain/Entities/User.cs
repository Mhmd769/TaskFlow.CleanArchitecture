using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
