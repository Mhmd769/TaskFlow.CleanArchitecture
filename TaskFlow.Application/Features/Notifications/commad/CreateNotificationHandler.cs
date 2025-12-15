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
    public class CreateNotificationCommandHandler
        : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly INotificationRepository _repo;
        private readonly INotificationService _notificationService;

        public CreateNotificationCommandHandler(
            INotificationRepository repo,
            INotificationService notificationService)
        {
            _repo = repo;
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken ct)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Message = request.Message,
                Link = request.Link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddRangeAsync(new[] { notification });
            await _repo.SaveAsync();

            // 🔥 THIS IS WHAT YOU WERE MISSING
            await _notificationService.SendNotificationToUser(
                request.UserId,
                notification
            );

            return notification.Id;
        }
    }


}
