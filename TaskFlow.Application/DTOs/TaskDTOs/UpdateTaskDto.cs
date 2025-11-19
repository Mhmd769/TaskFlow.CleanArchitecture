using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs.TaskDTOs
{
    public class UpdateTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public List<Guid> AssignedUserIds { get; set; } = new List<Guid>();
        public DateTime? DueDate { get; set; }
    }
}
