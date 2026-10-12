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
    public class FruitVarietyController(ISender mediator) : ControllerBase, IFruitVarietyController
    {
        [HttpGet("getVarieties")]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_READ)]
        [ProducesResponseType(typeof(IEnumerable<FruitVarietyDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<FruitVarietyDTO>> GetVarieties([FromQuery] int? fruitId)
        {
            return await mediator.Send(new GetFruitVarietiesQuery(fruitId));
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_READ)]
        [ProducesResponseType(typeof(PagedResult<FruitVarietyDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<FruitVarietyDTO>> GetVarietiesPaged([FromQuery] FruitVarietyFilterDTO filter)
        {
            return await mediator.Send(new GetFruitVarietiesPagedQuery(filter));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_READ)]
        [ProducesResponseType(typeof(FruitVarietyDTO),(int)HttpStatusCode.OK)]
        public async Task<FruitVarietyDTO> GetVarietyById(int id)
        {
            return await mediator.Send(new GetFruitVarietyByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_CREATE)]
        [ProducesResponseType(typeof(FruitVarietyDTO),(int)HttpStatusCode.Created)]
        public async Task<FruitVarietyDTO> CreateVariety([FromBody] SaveFruitVarietyDTO dto)
        {
            return await mediator.Send(new CreateFruitVarietyCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_UPDATE)]
        [ProducesResponseType(typeof(FruitVarietyDTO),(int)HttpStatusCode.OK)]
        public async Task<FruitVarietyDTO> UpdateVariety([FromBody] SaveFruitVarietyDTO dto)
        {
            return await mediator.Send(new UpdateFruitVarietyCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.FRUIT_VARIETY_DELETE)]
        [ProducesResponseType(typeof(bool),(int)HttpStatusCode.OK)]
        public async Task<bool> DeleteVariety(int id)
        {
            await mediator.Send(new DeleteFruitVarietyCommand(id));
            return true;
        }
    }
}
