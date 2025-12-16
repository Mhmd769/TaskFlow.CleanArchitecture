using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ChatDTOs;

namespace TaskFlow.Application.Common
{
    public interface IChatNotificationService
    {
        Task SendMessageToUser(string userId, MessageDto message);
    }
}
