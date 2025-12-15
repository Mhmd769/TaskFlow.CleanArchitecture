using System;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class CalendarTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
    }
}

