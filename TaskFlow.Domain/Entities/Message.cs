using System;

namespace TaskFlow.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        // ✅ Add this
        public Guid ConversationId { get; set; }
    }
}
