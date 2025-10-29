using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;
using System.Collections.Generic;

namespace TaskFlow.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUserQuery : IRequest<List<UserDto>>
    {
    }
}
