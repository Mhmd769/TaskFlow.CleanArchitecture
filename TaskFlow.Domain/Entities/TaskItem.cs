using System;
using System.Collections.Generic;
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

        // Many-to-many: Task can have multiple assigned users
        public ICollection<TaskAssignedUser> AssignedUsers { get; set; } = new List<TaskAssignedUser>();
    }

    public class TaskAssignedUser
    {
        public Guid TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
