using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

public class GetByIdProjectHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
        var mockMapper = new Mock<AutoMapper.IMapper>();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", FullName = "Test User" };

        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Description = "Test",
            OwnerId = userId,
            Owner = user,
            Tasks = new List<TaskItem>()
        };

        var projects = new List<Project> { project }.AsQueryable();

        // Mock Query() to support EF async
        mockProjectRepo.Setup(r => r.Query())
            .Returns(projects);

        mockMapper.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns((Project p) => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerName = p.Owner.FullName,
                TaskCount = p.Tasks.Count
            });

        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);

        var handler = new GetProjectByIdHandler(mockUnitOfWork.Object, mockMapper.Object);

        var query = new GetProjectByIdQuery
        {
            ProjectId = projectId
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.OwnerName.Should().Be("Test User");
        result.TaskCount.Should().Be(0);
    }
}
