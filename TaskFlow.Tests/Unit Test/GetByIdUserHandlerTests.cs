using Moq;
using Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Application.Features.Users.Queries.GetUserById;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Tests
{
    public class GetByIdUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            // -----------------------------
            // ARRANGE
            // -----------------------------
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "testuser", FullName = "Test User", Email = "test@mail.com" };

            mockUserRepo.Setup(r => r.GetByIdAsync(userId))
                        .ReturnsAsync(user);

            mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                      .Returns((User u) => new UserDto
                      {
                          Id = u.Id,
                          Username = u.Username,
                          FullName = u.FullName,
                          Email = u.Email
                      });

            var handler = new GetUserByIdHandler(mockUnitOfWork.Object, mockMapper.Object);

            var query = new GetUserByIdQuery
            {
                UserId = userId
            };

            // -----------------------------
            // ACT
            // -----------------------------
            var result = await handler.Handle(query, CancellationToken.None);

            // -----------------------------
            // ASSERT
            // -----------------------------
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Username.Should().Be("testuser");
            result.FullName.Should().Be("Test User");
            result.Email.Should().Be("test@mail.com");

            mockUserRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // -----------------------------
            // ARRANGE
            // -----------------------------
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

            var userId = Guid.NewGuid();

            mockUserRepo.Setup(r => r.GetByIdAsync(userId))
                        .ReturnsAsync((User?)null);

            var handler = new GetUserByIdHandler(mockUnitOfWork.Object, mockMapper.Object);

            var query = new GetUserByIdQuery
            {
                UserId = userId
            };

            // -----------------------------
            // ACT & ASSERT
            // -----------------------------
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(query, CancellationToken.None));

            mockUserRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }
    }
}
