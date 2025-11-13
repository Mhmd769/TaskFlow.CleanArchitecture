using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Application.Features.Users.Command.CreateUser;
using TaskFlow.Application.Features.Users.Queries.GetUserById;
using TaskFlow.Tests.Injection_Tools;
using Xunit;  // <— make sure this is included!

namespace TaskFlow.Tests.Integration_Test
{
    public class UserIntegrationTest : IClassFixture<SharedTestFixture>  // <-- FIX HERE
    {
        private readonly IMediator _mediator;
        private readonly SharedTestFixture _fixture;

        public UserIntegrationTest(SharedTestFixture fixture)
        {
            _fixture = fixture;
            _mediator = fixture.ServiceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Should_Create_And_Get_User_Successfully()
        {
            var userdto = new CreateUserDto
            {
                Username = "new_integration_user",
                FullName = "New Integration User",
                Email = "mhmdmahdi@gmail.com",
                PhoneNumber = "1234567890",
                Password = "1234mhmd",
            };

            var createcommand = new CreateUserCommand(userdto);
            var createduser = await _mediator.Send(createcommand);

            createduser.Should().NotBeNull();
            createduser.Username.Should().Be("new_integration_user");
            createduser.PhoneNumber.Should().Be("1234567890");
            createduser.Email.Should().Be("mhmdmahdi@gmail.com");

            var fetcheduser = await _mediator.Send(new GetUserByIdQuery
            {
                UserId = createduser.Id
            });

            fetcheduser.Should().NotBeNull();
            fetcheduser.Id.Should().Be(createduser.Id);
            fetcheduser.Username.Should().Be("new_integration_user");
        }
    }
}
