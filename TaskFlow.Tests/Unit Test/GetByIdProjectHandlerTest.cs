using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using Xunit;

public class GetByIdProjectHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMappedProject_WhenProjectExists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
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

        // 🟢 Instead of using Query() and EF async:
        mockProjectRepo.Setup(r => r.GetByIdAsync(project.Id))
                       .ReturnsAsync(project);

        mockMapper.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
                  .Returns(new ProjectDto
                  {
                      Id = project.Id,
                      Name = project.Name,
                      Description = project.Description,
                      OwnerName = project.Owner.FullName,
                      TaskCount = project.Tasks.Count
                  });

        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);

        var handler = new GetProjectByIdHandler(mockUnitOfWork.Object, mockMapper.Object);

        var query = new GetProjectByIdQuery { ProjectId = project.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.OwnerName.Should().Be("Test User");
    }
}