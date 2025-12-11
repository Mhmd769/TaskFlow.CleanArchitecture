using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;


namespace TaskFlow.Application.Features.Notifications.commad
{

    public record CreateNotificationCommand(
        string UserId,
        string Message,
        string? Link
    ) : IRequest<Guid>;

}
