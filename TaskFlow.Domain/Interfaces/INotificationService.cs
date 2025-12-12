using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
   public interface INotificationService
    {
        Task SendNotificationToUser(string userId, Notification notification);
    }
}
