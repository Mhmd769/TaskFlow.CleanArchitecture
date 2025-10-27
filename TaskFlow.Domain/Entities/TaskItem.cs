using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        // Relations
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
    }
}
