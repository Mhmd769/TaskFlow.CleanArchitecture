using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskFlow.Infrastructure.Notifications
{
    public class NotificationHub : Hub
    {
        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId != null)
        //        await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        //    await base.OnConnectedAsync();
        //}
    }

}
