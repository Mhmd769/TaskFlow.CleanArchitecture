using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Notifications.commad
{
    public class CreateNotificationHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly INotificationRepository _repo;

        public CreateNotificationHandler(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notif = new Notification
            {
                UserId = request.UserId,
                Message = request.Message,
                Link = request.Link
            };

            await _repo.AddRangeAsync((IEnumerable<Notification>)notif);
            await _repo.SaveAsync();

            return notif.Id;
        }
    }

}
