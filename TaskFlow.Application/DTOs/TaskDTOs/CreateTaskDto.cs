using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;


namespace TaskFlow.Application.DTOs.TaskDTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // Important: frontend sends ONLY user IDs
        public List<Guid> AssignedUserIds { get; set; } = new();
    }


}
