using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IWorkOrderRepository
    {
        Task<bool> AddWorkOrder(WorkOrderEntity workOrder);
        Task<IEnumerable<WorkOrderEntity>> DisplayListWorkOrder();
        Task<WorkOrderEntity> DisplayWorkOrderById(int orderId);
        Task<IEnumerable<WorkOrderEntity>> DisplayListWorkOrderByFarmId(int farmId);
        Task<IEnumerable<WorkOrderEntity>> DisplayListWorkOrderByTaskId(int taskId);
    }
}
