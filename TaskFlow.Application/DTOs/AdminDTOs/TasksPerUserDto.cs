using System;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class TasksPerUserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int TaskCount { get; set; }
    }
}

