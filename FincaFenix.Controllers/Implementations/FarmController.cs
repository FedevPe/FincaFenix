using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Farm;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.FARM_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class FarmController(ISender mediator) : ControllerBase, IFarmController
    {
        [HttpGet("getListFarm")]
        [ProducesResponseType(typeof(IEnumerable<FarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<FarmDTO>> GetListFarm()
        {
            return await mediator.Send(new GetFarmListQuery());
        }
        [HttpGet("getFarmById/{id}")]
        [ProducesResponseType(typeof(IEnumerable<FarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<FarmDTO> GetFarmById(int id)
        {
            return await mediator.Send(new GetFarmByIdQuery(id));
        }
    }
}
