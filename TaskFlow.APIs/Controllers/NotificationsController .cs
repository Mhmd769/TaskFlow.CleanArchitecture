using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.Features.Notifications.commad;
using TaskFlow.Application.Features.Notifications.Command;
using TaskFlow.Application.Features.Notifications.Queries.GetAllNotifications;
using TaskFlow.Application.Features.Notifications.Queries.GetUnreadNotifications;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }


        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User is not authenticated.");

            var notifications = await _mediator.Send(new GetUnreadNotificationsQuery(userIdClaim.Value));
            return Ok(notifications);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User is not authenticated.");

            var notifications = await _mediator.Send(new GetAllNotificationsQuery(userIdClaim.Value));
            return Ok(notifications);
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _mediator.Send(new MarkAsReadCommand(id));

            return result ? Ok("Marked as read") : NotFound("Notification not found");
        }

    }
}
