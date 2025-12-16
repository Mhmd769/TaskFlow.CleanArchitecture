using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ChatDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Notifications;

namespace TaskFlow.Infrastructure.Services
{
    // Infrastructure: implement interface
    public class ChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub> _hub;
        public ChatNotificationService(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }

        public async Task SendMessageToUser(string userId, MessageDto message)
        {
            // Use SignalR's built-in user mapping (based on NameIdentifier claim)
            await _hub.Clients
                .User(userId)
                .SendAsync("ReceiveMessage", message);
        }
    }

}
