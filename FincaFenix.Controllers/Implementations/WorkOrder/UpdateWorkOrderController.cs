using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers.UpdateWorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.WorkOrder
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateWorkOrderController (
        IUpdateWorkOrderInputPort interactor,
        IUpdateWorkOrderOutputPort presenter) : IUpdateWorkOrderController
    {
        [HttpPost("updatestateworkorder")]
        public async Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            await interactor.UpdateWorkOrderState(workOrderId, newStatus);
            return presenter.IsSuccesUpdate;
        }
    }
}
