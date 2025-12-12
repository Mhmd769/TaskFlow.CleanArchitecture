using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Notifications.Command
{
    public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, bool>
    {
        private readonly INotificationRepository _repo;

        public MarkAsReadHandler(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            // Load notification
            var notif = await _repo.GetByIdAsync(request.NotificationId);

            if (notif == null)
                return false;

            // Mark it as read
            notif.IsRead = true;

            await _repo.SaveAsync();
            return true;
        }
    }
}
