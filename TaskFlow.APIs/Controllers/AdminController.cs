using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetAdminDashboardStats;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetCalendarTasks;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksByStatus;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksOverTime;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerProject;
using TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerUser;

namespace TaskFlow.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _mediator.Send(new GetAdminDashboardStatsQuery());
            return Ok(stats);
        }

        [HttpGet("charts/tasks-by-status")]
        public async Task<IActionResult> GetTasksByStatus()
        {
            var data = await _mediator.Send(new GetTasksByStatusQuery());
            return Ok(data);
        }

        [HttpGet("charts/tasks-per-project")]
        public async Task<IActionResult> GetTasksPerProject()
        {
            var data = await _mediator.Send(new GetTasksPerProjectQuery());
            return Ok(data);
        }

        [HttpGet("charts/tasks-per-user")]
        public async Task<IActionResult> GetTasksPerUser()
        {
            var data = await _mediator.Send(new GetTasksPerUserQuery());
            return Ok(data);
        }

        [HttpGet("charts/tasks-over-time")]
        public async Task<IActionResult> GetTasksOverTime([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var data = await _mediator.Send(new GetTasksOverTimeQuery
            {
                StartDate = startDate,
                EndDate = endDate
            });

            return Ok(data);
        }

        [HttpGet("calendar/tasks")]
        public async Task<IActionResult> GetCalendarTasks([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var data = await _mediator.Send(new GetCalendarTasksQuery
            {
                StartDate = startDate,
                EndDate = endDate
            });

            return Ok(data);
        }
    }
}

