using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs.TaskDTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
