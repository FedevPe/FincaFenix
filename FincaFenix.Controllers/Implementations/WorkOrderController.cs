using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController(
        IWorkOrderInputPort interactor,
        IWorkOrderOutputPort presenter) : IWorkOrderController
    {
        [HttpPost("CreateWorkOrder")]
        public async Task<bool> CreateWorkOrder(WorkOrderDTO workOrder)
        {
            await interactor.Handle(workOrder);
            return presenter.IsSaved;
        }

        public Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrder(int quantity, int page, string state)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByFarmId(int farmId, int quantity, int page, string state)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByTaskId(int taskId, int quantity, int page, string state)
        {
            throw new NotImplementedException();
        }
    }
}
