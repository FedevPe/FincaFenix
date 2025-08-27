using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers.WorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.WorkOrder
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateWorkOrderController (
        ICreateWorkOrderInputPort interactor,
        ICreateWorkOrderOutputPort presenter): ICreateWorkOrderController
    {
        [HttpPost("createworkorder")]
        public async Task<bool> CreateWorkOrder(WorkOrderDTO workOrder)
        {
            await interactor.CreateWorkOrder(workOrder);
            return presenter.IsSaved;
        }
    }
}
