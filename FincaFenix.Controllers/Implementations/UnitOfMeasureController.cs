using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.UnitOfMeasure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UnitOfMeasureController(ISender mediator) : ControllerBase, IUnitOfMeasureController
    {
        [HttpGet]
        [Authorize(Policy = PolicyMaster.UNIT_OF_MEASURE_READ)]
        [ProducesResponseType(typeof(IEnumerable<UnitOfMeasureDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<UnitOfMeasureDTO>> GetUnitOfMeasureList()
        {
            return await mediator.Send(new GetUnitOfMeasureListQuery());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.UNIT_OF_MEASURE_READ)]
        [ProducesResponseType(typeof(UnitOfMeasureDTO),(int)HttpStatusCode.OK)]
        public async Task<UnitOfMeasureDTO> GetUnitOfMeasureById(int id)
        {
            return await mediator.Send(new GetUnitOfMeasureByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.UNIT_OF_MEASURE_CREATE)]
        [ProducesResponseType(typeof(UnitOfMeasureDTO),(int)HttpStatusCode.Created)]
        public async Task<UnitOfMeasureDTO> CreateUnitOfMeasure([FromBody] SaveUnitOfMeasureDTO dto)
        {
            return await mediator.Send(new CreateUnitOfMeasureCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.UNIT_OF_MEASURE_UPDATE)]
        [ProducesResponseType(typeof(UnitOfMeasureDTO),(int)HttpStatusCode.OK)]
        public async Task<UnitOfMeasureDTO> UpdateUnitOfMeasure([FromBody] SaveUnitOfMeasureDTO dto)
        {
            return await mediator.Send(new UpdateUnitOfMeasureCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.UNIT_OF_MEASURE_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteUnitOfMeasure(int id)
        {
            await mediator.Send(new DeleteUnitOfMeasureCommand(id));
            return true;
        }
    }
}
