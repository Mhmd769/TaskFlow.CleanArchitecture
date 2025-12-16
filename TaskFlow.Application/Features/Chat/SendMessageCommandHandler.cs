using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq; // Add this using directive at the top of the file
using System.Security.Claims;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ChatDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Chat
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationRepository _repo;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;


        public SendMessageCommandHandler(
            IMessageRepository messageRepo,
            IConversationRepository conversationRepo,
            IChatNotificationService chatNotificationService,
            IHttpContextAccessor httpContextAccessor,
            INotificationRepository repo,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _chatNotificationService = chatNotificationService;
            _httpContextAccessor = httpContextAccessor;
            _repo = repo;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            // 0️⃣ Ensure senderId is from JWT if empty
            if (string.IsNullOrEmpty(request.SenderId))
            {
                request.SenderId = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(request.SenderId))
                    throw new Exception("SenderId is missing. Make sure the user is authenticated.");
            }

            // 1️⃣ Get or create conversation
            var convo = await _conversationRepo.GetAsync(request.SenderId, request.ReceiverId);
            if (convo == null)
            {
                convo = new Conversation
                {
                    Id = Guid.NewGuid(),
                    User1Id = request.SenderId,
                    User2Id = request.ReceiverId,
                    LastMessageAt = DateTime.UtcNow
                };
                await _conversationRepo.AddAsync(convo);
            }

            // 2️⃣ Create message
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                ConversationId = convo.Id
            };
            await _messageRepo.AddAsync(message);

            // 3️⃣ Update last message in conversation
            await _conversationRepo.UpdateLastMessageAsync(convo.Id);

            // 3️⃣.b Create a notification for the receiver (for bell icon + real-time)
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.ReceiverId,
                Message = $"You have a new message from {request.SenderId}",
                Link = $"/chat/{request.SenderId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddRangeAsync(new[] { notification });
            await _repo.SaveAsync();

            // Push notification live via SignalR
            await _notificationService.SendNotificationToUser(notification.UserId, notification);


            // 4️⃣ Load sender full name
            var senderGuid = Guid.TryParse(message.SenderId, out var guid) ? guid : Guid.Empty;
            var senderUser = senderGuid != Guid.Empty
                ? await _unitOfWork.Users.GetByIdAsync(senderGuid)
                : null;
            var senderFullName = senderUser?.FullName ?? message.SenderId;

            // 5️⃣ Map to DTO
            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderFullName = senderFullName,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead
            };

            // 6️⃣ Send real-time via SignalR
            await _chatNotificationService.SendMessageToUser(request.ReceiverId, messageDto);

            return messageDto;
        }
    }
}
