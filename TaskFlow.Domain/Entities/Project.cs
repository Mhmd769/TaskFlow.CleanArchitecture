using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

