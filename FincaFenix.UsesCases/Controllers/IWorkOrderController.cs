using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IWorkOrderController
    {
        Task<bool> CreateWorkOrder(WorkOrderDTO workOrder);
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrder(int quantity, int page, string state);
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByFarmId(int farmId, int quantity, int page, string state);
        Task<IEnumerable<WorkOrderDTO>> DisplayListWorkOrderByTaskId(int taskId, int quantity, int page, string state);
    }
}
