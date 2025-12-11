using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Notifications.Queries.GetAllNotifications
{
    public record GetAllNotificationsQuery(string UserId) : IRequest<IEnumerable<Notification>>;

}
