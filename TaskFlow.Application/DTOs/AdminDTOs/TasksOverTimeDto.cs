using System;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class TasksOverTimeDto
    {
        public DateTime Date { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
    }
}

