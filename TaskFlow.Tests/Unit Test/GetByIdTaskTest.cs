using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Tests
{
    public class GetByIdTaskTest
    {
        [Fact]
        public async Task Handle_ShouldReturnMappedTask_WhenTaskExists()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTaskRepo = new Mock<IRepository<TaskItem>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            var user = new User { Id = Guid.NewGuid(), FullName = "Test User" };
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                Description = "Test Desc",
                Owner = user,
                Tasks = new List<TaskItem>()
            };
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Task Desc",
                AssignedUser = user,
                Project = project
            };

            mockTaskRepo.Setup(r => r.GetByIdAsync(task.Id))
                           .ReturnsAsync(task);

            mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
                .Returns(new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    AssignedUserId = task.AssignedUser.Id,
                    ProjectId = task.Project.Id
                });

            mockUnitOfWork.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

            var handler = new GetTaskByIdHandler(mockUnitOfWork.Object, mockMapper.Object);

            var query = new GetTaskByIdQuery { TaskId = task.Id };

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert

            result.Should().NotBeNull();
            result.Id.Should().Be(task.Id);
            result.AssignedUserId.Should().Be(user.Id);
            result.ProjectId.Should().Be(project.Id);

        }
    }
}