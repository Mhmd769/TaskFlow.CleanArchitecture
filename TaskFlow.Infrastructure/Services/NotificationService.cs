using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Notifications;

namespace TaskFlow.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task SendNotificationToUser(string userId, Notification notification)
        {
            await _hub.Clients
                .User(userId)
                .SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    userId = notification.UserId,
                    message = notification.Message,
                    link = notification.Link,
                    isRead = notification.IsRead,
                    createdAt = notification.CreatedAt
                });
        }
    }


}
