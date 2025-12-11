using Moq;
using Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Command.CreateProject;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

public class CreateProjectHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateProject_WhenCommandIsValid()
    {
        // -----------------------------
        // Arrange
        // -----------------------------
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockProjectRepo = new Mock<IRepository<Project>>();
        var mockUserRepo = new Mock<IRepository<User>>();
        var mockMapper = new Mock<AutoMapper.IMapper>();

        mockUnitOfWork.Setup(u => u.Projects).Returns(mockProjectRepo.Object);
        mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);
        mockUnitOfWork.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        mockProjectRepo.Setup(r => r.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

        // Mock AutoMapper behavior
        mockMapper.Setup(m => m.Map<Project>(It.IsAny<CreateProjectDto>()))
                  .Returns((CreateProjectDto dto) => new Project
                  {
                      Name = dto.Name,
                      Description = dto.Description,
                      OwnerId = dto.OwnerId
                  });

        mockMapper.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
                  .Returns((Project p) => new ProjectDto
                  {
                      Name = p.Name,
                      Description = p.Description
                  });

        // Setup owner
        var ownerId = Guid.NewGuid();
        mockUserRepo.Setup(u => u.GetByIdAsync(ownerId))
            .ReturnsAsync(new User { Id = ownerId, Username = "superadmin", FullName = "Super Admin" });

        // Create handler
       // var handler = new CreateProjectHandler(mockUnitOfWork.Object, mockMapper.Object);

        // Command DTO
        var dto = new CreateProjectDto
        {
            Name = "My Project",
            Description = "Testing",
            OwnerId = ownerId
        };
        var command = new CreateProjectCommand(dto);

      //  var result = await handler.Handle(command, CancellationToken.None);

        // -----------------------------
        // Assert
        // -----------------------------
       // result.Should().NotBeNull();
        //result.Name.Should().Be("My Project");

        mockProjectRepo.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);   
        mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }
}
