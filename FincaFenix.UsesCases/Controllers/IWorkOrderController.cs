using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IWorkOrderController
    {
        Task<bool> CreateWorkOrder(WorkOrderDTO workOrder);
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrder();
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByFarmId(int farmId);
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByTaskId(int taskId);
    }
}
