using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.DTOs.KafkaDTOs
{
    public class TaskAssignedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; }
        public Guid ProjectId { get; set; }
        public List<UserDto> AssignedUsers { get; set; } = new List<UserDto>();
    }
}
