using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Fruit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FruitController(ISender mediator) : ControllerBase, IFruitController
    {
        [HttpGet("getFruits")]
        [Authorize(Policy = PolicyMaster.FRUIT_READ)]
        [ProducesResponseType(typeof(IEnumerable<FruitDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<FruitDTO>> GetFruits()
        {
            return await mediator.Send(new GetFruitsQuery());
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.FRUIT_READ)]
        [ProducesResponseType(typeof(PagedResult<FruitDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<FruitDTO>> GetFruitsPaged([FromQuery] FruitFilterDTO filter)
        {
            return await mediator.Send(new GetFruitsPagedQuery(filter));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.FRUIT_READ)]
        [ProducesResponseType(typeof(FruitDTO),(int)HttpStatusCode.OK)]
        public async Task<FruitDTO> GetFruitById(int id)
        {
            return await mediator.Send(new GetFruitByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.FRUIT_CREATE)]
        [ProducesResponseType(typeof(FruitDTO),(int)HttpStatusCode.Created)]
        public async Task<FruitDTO> CreateFruit([FromBody] SaveFruitDTO dto)
        {
            return await mediator.Send(new CreateFruitCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.FRUIT_UPDATE)]
        [ProducesResponseType(typeof(FruitDTO),(int)HttpStatusCode.OK)]
        public async Task<FruitDTO> UpdateFruit([FromBody] SaveFruitDTO dto)
        {
            return await mediator.Send(new UpdateFruitCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.FRUIT_DELETE)]
        [ProducesResponseType(typeof(bool),(int)HttpStatusCode.OK)]
        public async Task<bool> DeleteFruit(int id)
        {
            await mediator.Send(new DeleteFruitCommand(id));
            return true;
        }
    }
}
