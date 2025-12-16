using System;

namespace TaskFlow.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string User1Id { get; set; } = null!;
        public string User2Id { get; set; } = null!;

        public DateTime LastMessageAt { get; set; }
    }
}
