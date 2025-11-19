using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;
using Xunit;

namespace TaskFlow.Tests
{
    public class GetByIdTaskTest
    {
        // ---------------------------------------
        // 1️⃣ RETURN FROM CACHE
        // ---------------------------------------
        [Fact]
        public async Task Handle_ShouldReturnTask_FromCache()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTaskRepo = new Mock<IRepository<TaskItem>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            var taskId = Guid.NewGuid();

            var cachedTask = new TaskDto
            {
                Id = taskId,
                Title = "Cached Task",
                Description = "From Cache"
            };

            mockCache.Setup(c => c.GetAsync<TaskDto>($"task:{taskId}"))
                     .ReturnsAsync(cachedTask);

            var handler = new GetTaskByIdHandler(
                mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

            var query = new GetTaskByIdQuery { TaskId = taskId };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Title.Should().Be("Cached Task");

            mockTaskRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        // ---------------------------------------
        // 2️⃣ RETURN FROM DB AND CACHE IT
        // ---------------------------------------
        [Fact]
        public async Task Handle_ShouldReturnMappedTask_WhenTaskExists_AndCacheIt()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTaskRepo = new Mock<IRepository<TaskItem>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                Description = "Test Desc"
            };

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Task Desc",
                Project = project
            };

            mockUnitOfWork.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

            mockCache.Setup(c => c.GetAsync<TaskDto>($"task:{task.Id}"))
                     .ReturnsAsync((TaskDto?)null);

            mockTaskRepo.Setup(r => r.GetByIdAsync(task.Id))
                        .ReturnsAsync(task);

            mockMapper.Setup(m => m.Map<TaskDto>(task))
                      .Returns(new TaskDto
                      {
                          Id = task.Id,
                          Title = task.Title,
                          Description = task.Description,
                          ProjectId = project.Id
                      });

            var handler = new GetTaskByIdHandler(
                mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

            var query = new GetTaskByIdQuery { TaskId = task.Id };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Id.Should().Be(task.Id);

            mockCache.Verify(c => c.SetAsync($"task:{task.Id}",
                                             It.IsAny<TaskDto>(),
                                             It.IsAny<TimeSpan>()), Times.Once);
        }

        // ---------------------------------------
        // 3️⃣ TASK NOT FOUND → THROW EXCEPTION
        // ---------------------------------------
        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTaskRepo = new Mock<IRepository<TaskItem>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            var taskId = Guid.NewGuid();

            mockUnitOfWork.Setup(u => u.Tasks).Returns(mockTaskRepo.Object);

            mockCache.Setup(c => c.GetAsync<TaskDto>($"task:{taskId}"))
                     .ReturnsAsync((TaskDto?)null);

            mockTaskRepo.Setup(r => r.GetByIdAsync(taskId))
                        .ReturnsAsync((TaskItem?)null);

            var handler = new GetTaskByIdHandler(
                mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

            var query = new GetTaskByIdQuery { TaskId = taskId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}
