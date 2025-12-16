using MediatR;
using TaskFlow.Application.DTOs.ChatDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Chat
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepo;

        public GetConversationQueryHandler(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
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
            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            });
        }
    }
}
