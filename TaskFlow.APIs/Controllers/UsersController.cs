using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Application.Features.Users.Command.CreateUser;
using TaskFlow.Application.Features.Users.Command.DeleteUser;
using TaskFlow.Application.Features.Users.Command.UpdateUser;
using TaskFlow.Application.Features.Users.Queries.GetAllUsers;
using TaskFlow.Application.Features.Users.Queries.GetUserById;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _mediator.Send(new GetAllUserQuery());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var command = new CreateUserCommand(dto); // assign User properly
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
        {
            var command = new UpdateUserCommand(dto);
            var user = await _mediator.Send(command);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _mediator.Send(new DeleteUserCommand { userid = id });
            return Ok(user);
        }
    }

}
