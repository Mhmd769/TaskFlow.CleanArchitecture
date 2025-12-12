using MediatR;
using System;

namespace TaskFlow.Application.Features.Notifications.Command
{
    public class MarkAsReadCommand : IRequest<bool>
    {
        public Guid NotificationId { get; set; }

        public MarkAsReadCommand(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
