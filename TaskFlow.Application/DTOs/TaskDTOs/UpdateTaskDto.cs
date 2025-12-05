using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;


namespace TaskFlow.Application.DTOs.TaskDTOs

{
    public class UpdateTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }  // ← ADD THIS LINE
        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public List<Guid> AssignedUserIds { get; set; } = new();
    }

}
