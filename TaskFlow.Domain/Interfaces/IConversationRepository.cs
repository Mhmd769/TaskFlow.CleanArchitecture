using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetAsync(string user1Id, string user2Id);
        Task AddAsync(Conversation conversation);
        Task UpdateLastMessageAsync(Guid conversationId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
    }
}
