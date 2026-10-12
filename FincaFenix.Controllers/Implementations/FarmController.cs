using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Farm;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FarmController(ISender mediator) : ControllerBase, IFarmController
    {
        [HttpGet("getListFarm")]
        [Authorize(Policy = PolicyMaster.FARM_READ)]
        [ProducesResponseType(typeof(IEnumerable<FarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<FarmDTO>> GetListFarm()
        {
            return await mediator.Send(new GetFarmListQuery());
        }

        [HttpGet("getFarmById/{id}")]
        [Authorize(Policy = PolicyMaster.FARM_READ)]
        [ProducesResponseType(typeof(IEnumerable<FarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<FarmDTO> GetFarmById(int id)
        {
            return await mediator.Send(new GetFarmByIdQuery(id));
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.FARM_READ)]
        [ProducesResponseType(typeof(PagedResult<FarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<FarmDTO>> GetFarmListPaged([FromQuery] FarmFilterDTO filter)
        {
            return await mediator.Send(new GetFarmListPagedQuery(filter));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.FARM_CREATE)]
        [ProducesResponseType(typeof(FarmDTO),(int)HttpStatusCode.Created)]
        public async Task<FarmDTO> CreateFarm([FromBody] SaveFarmDTO dto)
        {
            return await mediator.Send(new CreateFarmCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.FARM_UPDATE)]
        [ProducesResponseType(typeof(FarmDTO),(int)HttpStatusCode.OK)]
        public async Task<FarmDTO> UpdateFarm([FromBody] SaveFarmDTO dto)
        {
            return await mediator.Send(new UpdateFarmCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.FARM_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteFarm(int id)
        {
            await mediator.Send(new DeleteFarmCommand(id));
            return true;
        }

        [HttpPut("ReactivateFarm/{id}")]
        [Authorize(Policy = PolicyMaster.FARM_UPDATE)]
        [ProducesResponseType(typeof(FarmDTO),(int)HttpStatusCode.OK)]
        public async Task<FarmDTO> ReactivateFarm(int id)
        {
            return await mediator.Send(new ReactivateFarmCommand(id));
        }
    }
}