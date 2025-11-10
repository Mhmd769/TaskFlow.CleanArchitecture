using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Application.Features.Users.Command.CreateUser;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Tests
{
    public class CreateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();


            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);
            mockUnitOfWork.Setup(u => u.SaveAsync()).ReturnsAsync(1);
            mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Mock AutoMapper behavior

            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserDto>()))
                      .Returns((CreateUserDto dto) => new User
                      {
                          Username = dto.Username,
                          FullName = dto.FullName,
                          Email = dto.Email
                      });
            mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                        .Returns((User u) => new UserDto
                        {
                            Username = u.Username,
                            FullName = u.FullName,
                            Email = u.Email
                        });


            // Create handler

            var handler = new CreateUserHandler(mockUnitOfWork.Object, mockMapper.Object);
            // Command DTO
            var dto = new CreateUserDto
            {
                Username = "johndoe",
                FullName = "John Doe",
                PhoneNumber = "1234567890",
                Email = "alo12@gmail.com",
                Password = "Password@123"

            };

            var command = new CreateUserCommand(dto);

            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            result.FullName.Should().Be(dto.FullName);

            mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);


        }
    }
}

