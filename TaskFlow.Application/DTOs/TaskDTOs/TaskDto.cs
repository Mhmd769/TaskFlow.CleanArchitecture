using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;


namespace TaskFlow.Application.DTOs.TaskDTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;

        // Many-to-many result:
        public List<Guid> AssignedUserIds { get; set; } = new();
        public List<string> AssignedUserNames { get; set; } = new();
    }

}
