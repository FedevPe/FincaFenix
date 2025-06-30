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

        public async Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrder(int farmId, string state = "Activo")
        {
            await interactor.GetWorkOrderList(farmId, state);
            return presenter.WorkOrderList;
        }

        public Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByFarmId(int farmId, string state)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByTaskId(int taskId, int quantity, int page, string state)
        {
            throw new NotImplementedException();
        }
    }
}
