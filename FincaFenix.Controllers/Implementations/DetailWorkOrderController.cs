using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetailWorkOrderController (
        IAddDetailWorkOrderInputPort interactor,
        IDetailWorkOrderOutputPort presenter) : IDetailWorkOrderController
    {
        [HttpPost("addDetailWO")]
        public async Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto)
        {
            await interactor.AddDetailWorkOrder(dto);
            return presenter.IsSuccess;
        }
        [HttpGet("order/{orderId}/getDetail")]
        public async Task<IEnumerable<DetailWorkOrderDTO>> GetActivitiesByOrderId(int orderId)
        {
            await interactor.GetActivitiesByOrderId(orderId);
            return presenter.ActivityLog;
        }
    }
}
