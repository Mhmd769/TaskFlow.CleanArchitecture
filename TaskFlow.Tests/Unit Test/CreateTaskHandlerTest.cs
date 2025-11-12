using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.Features.Tasks.Command.CreateTask;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TaskFlow.Tests
{
    public class CreateTaskHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldCreateTask_WhenCommandIsValid()
        {
            // -----------------------------
            // Arrange
            // -----------------------------
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockProjectRepo = new Mock<IRepository<Project>>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockTaskRepo = new Mock<IRepository<TaskItem>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);
            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);
            mockUnitOfWork.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);
            mockUnitOfWork.Setup(u => u.SaveAsync()).ReturnsAsync(1);
            mockProjectRepo.Setup(r => r.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

            mockMapper.Setup(m=>m.Map<TaskItem>(It.IsAny<CreateTaskDto>()))
                .Returns((CreateTaskDto dto) => new TaskItem
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    AssignedUserId = dto.AssignedUserId,
                    ProjectId = dto.ProjectId
                });

            mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
                .Returns((TaskItem t) => new TaskDto
                {
                    Title = t.Title,
                    Description = t.Description,
                    AssignedUserId = t.AssignedUserId,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId
                });


            var ownerId = Guid.NewGuid();
            // Setup owner
            mockUserRepo.Setup(u => u.GetByIdAsync(ownerId))
                .ReturnsAsync(new User { Id = ownerId, Username = "mhmd23", FullName = "mhmd mahdi" });

            var projectId = Guid.NewGuid();
            mockProjectRepo.Setup(p => p.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId, Name = "Project X", Description = "Top Secret", OwnerId = ownerId });

            var handler = new CreateTaskHandler(mockUnitOfWork.Object, mockMapper.Object);

            var dto = new CreateTaskDto
            {
                Title = "Task 1",
                Description = "Task Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = projectId,
                AssignedUserId = ownerId
            };
            var command = new CreateTaskCommand(dto);

            var result = await handler.Handle(command, CancellationToken.None);


            result.Should().NotBeNull();
            result.Title.Should().Be("Task 1");
            result.AssignedUserId.Should().Be(ownerId);
            result.ProjectId.Should().Be(projectId);

            mockTaskRepo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);

        }
    }
}
