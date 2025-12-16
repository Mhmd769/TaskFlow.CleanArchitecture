using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<IEnumerable<Message>> GetConversationAsync(string userId, string otherUserId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(Guid messageId);
        Task MarkConversationAsReadAsync(string userId, string otherUserId);
    }
}
