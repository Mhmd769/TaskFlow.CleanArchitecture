using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.Features.Chat;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChatController(IMediator mediator) => _mediator = mediator;

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(SendMessageCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet("{otherUserId}")]
        public async Task<IActionResult> GetConversation(string otherUserId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetConversationQuery(userId, otherUserId);
            return Ok(await _mediator.Send(query));
        }
    }

}
