using System;

namespace TaskFlow.Application.DTOs.AdminDTOs
{
    public class AdminDashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }

        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int CancelledTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}

