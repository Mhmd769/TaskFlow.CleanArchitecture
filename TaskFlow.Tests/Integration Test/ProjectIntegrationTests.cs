using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Command.CreateProject;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Tests.Injection_Tools;
using Xunit;

namespace TaskFlow.Tests.Integration_Test
{
    public class ProjectIntegrationTests : IClassFixture<SharedTestFixture>
    {
        private readonly IMediator _mediator;
        private readonly SharedTestFixture _fixture;

        public ProjectIntegrationTests(SharedTestFixture fixture)
        {
            _fixture = fixture;
            _mediator = fixture.ServiceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Should_Create_And_Get_Project_Successfully()
        {
            // Arrange
            var userId = _fixture.DefaultUserId;
            var createDto = new CreateProjectDto
            {
                Name = "Integration Test Project",       
                Description = "Testing real db flow",
                OwnerId = userId
            };

            var createCommand = new CreateProjectCommand(createDto);

            // Act: create project
            var createdProject = await _mediator.Send(createCommand);

            // Assert create
            createdProject.Should().NotBeNull();
            createdProject.Name.Should().Be("Integration Test Project");

            // Act: fetch project by Id
            var fetched = await _mediator.Send(new GetProjectByIdQuery
            {
                ProjectId = createdProject.Id
            });

            // Assert fetch
            fetched.Should().NotBeNull();
            fetched.Id.Should().Be(createdProject.Id);
            fetched.Name.Should().Be("Integration Test Project");
        }
    }
}
