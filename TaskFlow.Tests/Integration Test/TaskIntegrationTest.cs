using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.Features.Projects.Command.CreateProject;
using TaskFlow.Application.Features.Tasks.Command.CreateTask;
using TaskFlow.Tests.Injection_Tools;

namespace TaskFlow.Tests.Integration_Test
{
    public class TaskIntegrationTest : IClassFixture<SharedTestFixture>
    {
        private readonly IMediator _mediator;
        private readonly SharedTestFixture _fixture;

        public TaskIntegrationTest(SharedTestFixture fixture)
        {
            _fixture = fixture;
            _mediator = fixture.ServiceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Should_Create_And_Get_Task_Successfully()
        {
            var userId = _fixture.DefaultUserId;
            var projectDto = new CreateProjectDto
            {
                Name = "Task Test Project",
                Description = "Project for Task Testing",
                OwnerId = userId
            };

            var createproject = new CreateProjectCommand(projectDto);

            var createdProject = await _mediator.Send(createproject);

            var taskdto = new CreateTaskDto
            {
                Title = "Integration Test Task",
                Description = "Testing task creation",
                ProjectId = createdProject.Id,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var createTaskCommand = new CreateTaskCommand(taskdto);
            var createdTask = await _mediator.Send(createTaskCommand);

            createdTask.Should().NotBeNull();
            createdTask.Title.Should().Be("Integration Test Task");
            createdTask.ProjectId.Should().Be(createdProject.Id);

        }
    }
}
