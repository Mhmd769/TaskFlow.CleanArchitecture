using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Notifications.Queries.GetAllNotifications
{
    public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<Notification>>
    {
        private readonly INotificationRepository _repo;

        public GetAllNotificationsHandler(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(request.UserId);
        }
    }
}
