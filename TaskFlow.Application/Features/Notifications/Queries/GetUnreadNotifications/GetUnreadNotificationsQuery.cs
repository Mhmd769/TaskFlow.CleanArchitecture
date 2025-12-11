using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Notifications.Queries.GetUnreadNotifications
{
    public record GetUnreadNotificationsQuery(string UserId)
        : IRequest<IEnumerable<Notification>>;
}
