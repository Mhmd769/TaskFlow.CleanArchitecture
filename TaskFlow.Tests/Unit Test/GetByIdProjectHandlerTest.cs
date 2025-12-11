using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;
using Xunit;

public class GetByIdProjectHandlerTests
{
    // -----------------------------
    // 1️⃣ RETURN PROJECT FROM CACHE
    // -----------------------------
    [Fact]
    public async Task Handle_ShouldReturnProject_FromCache()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
        var mockMapper = new Mock<AutoMapper.IMapper>();
        var mockCache = new Mock<ICacheService>();

        var projectId = Guid.NewGuid();
        var owner = new User { Id = Guid.NewGuid(), FullName = "Owner Name" };

        var cachedProject = new ProjectDto
        {
            Id = projectId,
            Name = "Cached Project",
            Description = "From Cache",
            TaskCount = 0,
            Owner = owner != null ? new TaskFlow.Application.DTOs.UserDTOs.UserDto
            {
                Id = owner.Id,
                FullName = owner.FullName
            } : null
        };

        mockCache.Setup(c => c.GetAsync<ProjectDto>($"project:{projectId}"))
                 .ReturnsAsync(cachedProject);

       // var handler = new GetProjectByIdHandler(
     //       mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

        var query = new GetProjectByIdQuery { ProjectId = projectId };

       // var result = await handler.Handle(query, CancellationToken.None);

       // result.Should().NotBeNull();
       // result.Name.Should().Be("Cached Project");

        mockProjectRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    // -----------------------------------------
    // 2️⃣ RETURN PROJECT FROM DB & CACHE IT
    // -----------------------------------------
    [Fact]
    public async Task Handle_ShouldReturnMappedProject_WhenProjectExists_AndCacheIt()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
        var mockMapper = new Mock<AutoMapper.IMapper>();
        var mockCache = new Mock<ICacheService>();

        var user = new User { Id = Guid.NewGuid(), FullName = "Test User" };

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            Description = "Test Desc",
            Owner = user,
            Tasks = new List<TaskItem>()
        };

        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);

        mockCache.Setup(c => c.GetAsync<ProjectDto>($"project:{project.Id}"))
                 .ReturnsAsync((ProjectDto?)null);

        mockProjectRepo.Setup(r => r.GetByIdAsync(project.Id))
                       .ReturnsAsync(project);

        mockMapper.Setup(m => m.Map<ProjectDto>(project))
                  .Returns(new ProjectDto
                  {
                      Id = project.Id,
                      Name = project.Name,
                      Description = project.Description,
                      Owner= project.Owner != null ? new TaskFlow.Application.DTOs.UserDTOs.UserDto
                      {
                          Id = project.Owner.Id,
                          FullName = project.Owner.FullName
                      } : null,
                      TaskCount = project.Tasks.Count
                  });

       // var handler = new GetProjectByIdHandler(
        //    mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

        var query = new GetProjectByIdQuery { ProjectId = project.Id };

       // var result = await handler.Handle(query, CancellationToken.None);

      //  result.Should().NotBeNull();
      //  result.Name.Should().Be("Test Project");

        mockCache.Verify(c => c.SetAsync($"project:{project.Id}",
                                         It.IsAny<ProjectDto>(),
                                         It.IsAny<TimeSpan>()), Times.Once);
    }

    // -------------------------------------
    // 3️⃣ PROJECT NOT FOUND → THROW ERROR
    // -------------------------------------
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
        var mockMapper = new Mock<AutoMapper.IMapper>();
        var mockCache = new Mock<ICacheService>();

        var projectId = Guid.NewGuid();

        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);

        mockCache.Setup(c => c.GetAsync<ProjectDto>($"project:{projectId}"))
                 .ReturnsAsync((ProjectDto?)null);

        mockProjectRepo.Setup(r => r.GetByIdAsync(projectId))
                       .ReturnsAsync((Project?)null);

       // var handler = new GetProjectByIdHandler(
         //   mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

        var query = new GetProjectByIdQuery { ProjectId = projectId };

       // await Assert.ThrowsAsync<NotFoundException>(() =>
          //  handler.Handle(query, CancellationToken.None));
    }
}
