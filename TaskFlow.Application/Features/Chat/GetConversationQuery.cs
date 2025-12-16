using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ChatDTOs;

namespace TaskFlow.Application.Features.Chat
{
    public class GetConversationQuery : IRequest<IEnumerable<MessageDto>>
    {
        public GetConversationQuery(string? userId, string otherUserId)
        {
            UserId = userId;
            OtherUserId = otherUserId;
        }

        public string UserId { get; set; } = string.Empty;
        public string OtherUserId { get; set; } = string.Empty;
    }
}