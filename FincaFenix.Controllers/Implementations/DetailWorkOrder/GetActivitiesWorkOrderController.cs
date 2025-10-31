using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.DetailWorkOrder
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetActivitiesWorkOrderController (
        IGetActivitiesWorkOrderInputPort interactor,
        IGetActivitiesWorkOrderOutputPort presenter) : IGetActivitiesWorkOrderController
    {
        [HttpGet("order/{orderId}/getDetail")]
        public async Task<IEnumerable<ActivityWorkOrderDTO>> GetActivitiesByOrderId(int orderId)
        {
            await interactor.GetActivitiesByOrderId(orderId);
            return presenter.ActivityLog;
        }
    }
}
