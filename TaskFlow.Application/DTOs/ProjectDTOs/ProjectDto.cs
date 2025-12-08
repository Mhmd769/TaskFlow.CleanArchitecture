using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.DTOs.ProjectDTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Display-only fields
        public UserDto Owner { get; set; }
        public List<TaskDto>? Tasks { get; set; } = null; 
        public int TaskCount { get; set; }
    }
}
