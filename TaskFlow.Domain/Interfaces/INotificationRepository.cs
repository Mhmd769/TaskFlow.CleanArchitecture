using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task<IEnumerable<Notification>> GetUnreadAsync(string userId);
        Task<IEnumerable<Notification>> GetAllAsync(string userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task SaveAsync();
    }

}
