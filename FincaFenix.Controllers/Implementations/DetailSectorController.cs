using System.ComponentModel;
using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.DetailSector;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.SECTOR_FARM_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class DetailSectorController(ISender mediator) : ControllerBase, IDetailSectorController
    {
        [HttpGet("farm/{farmId}/sectors")]
        [ProducesResponseType(typeof(IEnumerable<DetailSectorFarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByFarmId(int farmId, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetSectorListByFarmQuery(farmId), cancellationToken);
        }
        [HttpGet("order/{orderId}/sectors")]
        [ProducesResponseType(typeof(IEnumerable<DetailSectorFarmDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByOrderId(int orderId, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetSectorListByOrderQuery(orderId), cancellationToken);
        }
    }
}