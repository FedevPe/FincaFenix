using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ISender mediator) : ControllerBase, IUserController
    {
        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.USER_READ)]
        [ProducesResponseType(typeof(PagedResult<UserDTO>), (int)HttpStatusCode.OK)]
        public async Task<PagedResult<UserDTO>> GetUserListPaged([FromQuery] UserFilterDTO filter)
        {
            return await mediator.Send(new GetUserListPagedQuery(filter));
        }

        [HttpGet("roles")]
        [Authorize(Policy = PolicyMaster.USER_READ)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<string>> GetRoles()
        {
            return await mediator.Send(new GetRolesQuery());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.USER_READ)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<UserDTO> GetUserById(string id)
        {
            return await mediator.Send(new GetUserByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.USER_CREATE)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)]
        public async Task<UserDTO> CreateUser([FromBody] CreateUserDTO dto)
        {
            return await mediator.Send(new CreateUserCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.USER_UPDATE)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<UserDTO> UpdateUser([FromBody] UpdateUserDTO dto)
        {
            return await mediator.Send(new UpdateUserCommand(dto));
        }

        [HttpPut("DeactivateUser/{id}")]
        [Authorize(Policy = PolicyMaster.USER_UPDATE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeactivateUser(string id)
        {
            await mediator.Send(new DeactivateUserCommand(id));
            return true;
        }

        [HttpPut("ReactivateUser/{id}")]
        [Authorize(Policy = PolicyMaster.USER_UPDATE)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<UserDTO> ReactivateUser(string id)
        {
            return await mediator.Send(new ReactivateUserCommand(id));
        }
    }
}