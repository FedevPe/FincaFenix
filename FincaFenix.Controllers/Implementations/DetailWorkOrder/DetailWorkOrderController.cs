using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;
using FincaFenix.UsesCases.UseCases.DetailWorkOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.DetailWorkOrder
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DetailWorkOrderController(IMediator mediator) : IAddDetailWorkOrderController, IGetActivitiesWorkOrderController
    {
        [HttpPost("addDetailWO")]
        [Authorize(Policy = PolicyMaster.DETAIL_WORKORDER_CREATE)]
        public async Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto)
        {
            return await mediator.Send(new AddDetailWorkOrderCommand(dto));
        }

        [HttpGet("order/{orderId}/getDetail")]
        [Authorize(Policy = PolicyMaster.DETAIL_WORKORDER_READ)]
        public async Task<IEnumerable<ActivityWorkOrderDTO>> GetActivitiesByOrderId(int orderId)
        {
            return await mediator.Send(new GetActivitiesByOrderIdQuery(orderId));
        }
    }
}
