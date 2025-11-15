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
using TaskFlow.Application.Common;

namespace TaskFlow.Tests
{
    public class GetUserByIdHandlerTests
    {
        // ---------------------------
        // 1️⃣ Should Return from Cache
        // ---------------------------
        [Fact]
        public async Task Handle_ShouldReturnUser_FromCache()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

            var userId = Guid.NewGuid();
            var cachedUserDto = new UserDto
            {
                Id = userId,
                Username = "cachedUser",
                Email = "cached@mail.com",
                FullName = "Cached User"
            };

            mockCache.Setup(c => c.GetAsync<UserDto>($"user:{userId}"))
                     .ReturnsAsync(cachedUserDto);

            var handler = new GetUserByIdHandler(mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);
            var query = new GetUserByIdQuery { UserId = userId };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Username.Should().Be("cachedUser");

            mockUserRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        // ---------------------------------------------
        // 2️⃣ Should Return from DB and Save into Cache
        // ---------------------------------------------
        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists_AndCacheIt()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "dbuser", Email = "db@mail.com", FullName = "DB User" };

            mockCache.Setup(c => c.GetAsync<UserDto>($"user:{userId}"))
                     .ReturnsAsync((UserDto?)null);

            mockUserRepo.Setup(r => r.GetByIdAsync(userId))
                        .ReturnsAsync(user);

            mockMapper.Setup(m => m.Map<UserDto>(user))
                      .Returns(new UserDto
                      {
                          Id = userId,
                          Username = "dbuser",
                          Email = "db@mail.com",
                          FullName = "DB User"
                      });

            var handler = new GetUserByIdHandler(mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);
            var query = new GetUserByIdQuery { UserId = userId };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Username.Should().Be("dbuser");

            mockCache.Verify(c => c.SetAsync($"user:{userId}", It.IsAny<UserDto>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        // ----------------------------------------
        // 3️⃣ Should Throw NotFoundException
        // ----------------------------------------
        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IRepository<User>>();
            var mockMapper = new Mock<AutoMapper.IMapper>();
            var mockCache = new Mock<ICacheService>();

            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepo.Object);

            var userId = Guid.NewGuid();

            mockCache.Setup(c => c.GetAsync<UserDto>($"user:{userId}"))
                     .ReturnsAsync((UserDto?)null);

            mockUserRepo.Setup(r => r.GetByIdAsync(userId))
                        .ReturnsAsync((User?)null);

            var handler = new GetUserByIdHandler(mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);
            var query = new GetUserByIdQuery { UserId = userId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}
