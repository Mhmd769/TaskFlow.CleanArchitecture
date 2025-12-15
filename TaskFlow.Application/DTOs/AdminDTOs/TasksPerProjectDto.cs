using System;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class TasksPerProjectDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public int TaskCount { get; set; }
    }
}

