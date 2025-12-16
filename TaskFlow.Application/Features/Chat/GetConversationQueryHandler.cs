using MediatR;
using TaskFlow.Application.DTOs.ChatDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Chat
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUnitOfWork _unitOfWork;

        public GetConversationQueryHandler(IMessageRepository messageRepo, IUnitOfWork unitOfWork)
        {
            _messageRepo = messageRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            // Mark all messages that this user has received from the other user as read
            if (!string.IsNullOrEmpty(request.UserId) && !string.IsNullOrEmpty(request.OtherUserId))
            {
                await _messageRepo.MarkConversationAsReadAsync(request.UserId, request.OtherUserId);
            }

            // Then load the updated conversation
            var messages = await _messageRepo.GetConversationAsync(request.UserId, request.OtherUserId);

            // Preload all sender users to avoid N+1
            var senderIds = messages.Select(m => m.SenderId)
                                    .Distinct()
                                    .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
                                    .Where(g => g != Guid.Empty)
                                    .ToList();

            var usersQuery = _unitOfWork.Users.GetAll()
                               .Where(u => senderIds.Contains(u.Id));
            var users = await Task.FromResult(usersQuery.ToList());

            return messages.Select(m =>
            {
                var senderGuid = Guid.TryParse(m.SenderId, out var g) ? g : Guid.Empty;
                var senderUser = users.FirstOrDefault(u => u.Id == senderGuid);

                return new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderFullName = senderUser?.FullName ?? m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead
                };
            });
        }
    }
}
