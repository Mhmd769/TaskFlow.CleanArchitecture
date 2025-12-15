using System;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class TasksByStatusDto
    {
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Overdue { get; set; }
    }
}

