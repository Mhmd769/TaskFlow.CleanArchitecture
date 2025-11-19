using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.DTOs.TaskDTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime? DueDate { get; set; }

        public List<AssignedUserDto> AssignedUsers { get; set; } = new();
    }

    public class AssignedUserDto
    {
        public Guid Id { get; set; }
        // you can keep other fields for convenience, but backend only needs Id
    }

}
