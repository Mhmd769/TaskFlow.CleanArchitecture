using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Notifications.Queries.GetUnreadNotifications
{
    public class GetUnreadNotificationsHandler : IRequestHandler<GetUnreadNotificationsQuery, IEnumerable<Notification>>
    {
        private readonly INotificationRepository _repo;

        public GetUnreadNotificationsHandler(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Notification>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetUnreadAsync(request.UserId);
        }
    }

}
