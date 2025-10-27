using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
